using GrooveOn.MailingService.Configuration;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.PaymentStateMachine;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


namespace GrooveOn.Services.Services
{
    public class PaymentService
        : BaseCRUDService<PaymentResponse, PaymentSearchObject, Payment, PaymentUpsertRequest, PaymentUpsertRequest>,
          IPaymentService
    {
        private readonly IStripeService _stripeService;
        private readonly AppConfig _config;
        private readonly BasePaymentState _paymentState;

        public PaymentService(
            GrooveOnDbContext context,
            IMapper mapper,
            IStripeService stripeService,
            IOptions<AppConfig> config,
            BasePaymentState paymentState
        ) : base(context, mapper)
        {
            _stripeService = stripeService;
            _config = config.Value;
            _paymentState = paymentState;
        }

        protected override IQueryable<Payment> ApplyFilter(
            IQueryable<Payment> query,
            PaymentSearchObject search)
        {
            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.Trim().ToLower();

                query = query.Where(x =>
                    x.PaymentStatus.ToLower().Contains(fts)
                    || (x.StripePaymentIntentId != null &&
                        x.StripePaymentIntentId.ToLower().Contains(fts))
                    || (x.FailureReason != null &&
                        x.FailureReason.ToLower().Contains(fts))
                    || (x.PaymentMethod != null &&
                        x.PaymentMethod.ToLower().Contains(fts))
                    || x.PaymentAmount.ToString().Contains(fts)
                );
            }

            if (search.SubscriptionId.HasValue)
                query = query.Where(x => x.SubscriptionId == search.SubscriptionId.Value);

            if (search.UserId.HasValue)
                query = query.Where(x =>
                    x.Subscription != null &&
                    x.Subscription.UserId == search.UserId.Value);

            if (!string.IsNullOrWhiteSpace(search.PaymentStatus))
                query = query.Where(x => x.PaymentStatus == search.PaymentStatus);

            if (!string.IsNullOrWhiteSpace(search.StripePaymentIntentId))
                query = query.Where(x => x.StripePaymentIntentId == search.StripePaymentIntentId);

            return base.ApplyFilter(query, search);
        }

        protected override IQueryable<Payment> AddInclude(
            IQueryable<Payment> query,
            PaymentSearchObject search)
        {
            query = query
                .Include(x => x.Subscription)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subscription)
                    .ThenInclude(x => x.SubscriptionPlan);

            return base.AddInclude(query, search);
        }

        protected override PaymentResponse MapToResponse(Payment entity)
        {
            var response = _mapper.Map<PaymentResponse>(entity);

            response.UserId = entity.Subscription?.UserId;
            response.Username = entity.Subscription?.User?.Username;

            response.SubscriptionPlanId = entity.Subscription?.SubscriptionPlanId;
            response.SubscriptionPlanName = entity.Subscription?.SubscriptionPlan?.Name;

            return response;
        }

        protected override async Task BeforeInsert(Payment entity, PaymentUpsertRequest request)
        {
            entity.PaymentStatus = string.IsNullOrWhiteSpace(entity.PaymentStatus)
                ? "Pending"
                : entity.PaymentStatus;

            entity.CreatedAt = entity.CreatedAt == default
                ? DateTime.UtcNow
                : entity.CreatedAt;

            if (string.IsNullOrWhiteSpace(entity.PaymentMethod))
                entity.PaymentMethod = "Stripe";

            if (entity.PaymentStatus == "Paid")
            {
                entity.PaidAt ??= DateTime.UtcNow;
                entity.PaymentDate ??= DateTime.UtcNow;
            }
            else
            {
                entity.PaidAt = null;
                entity.PaymentDate = null;
            }

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(Payment entity, PaymentUpsertRequest request)
        {
            if (string.IsNullOrWhiteSpace(entity.PaymentMethod))
                entity.PaymentMethod = "Stripe";

            if (entity.PaymentStatus == "Paid")
            {
                entity.PaidAt ??= DateTime.UtcNow;
                entity.PaymentDate ??= DateTime.UtcNow;
            }
            else
            {
                entity.PaidAt = null;
                entity.PaymentDate = null;
            }

            await base.BeforeUpdate(entity, request);
        }

        public async Task<object> CreateNewPaymentIntentAsync(CreatePaymentIntentRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId);

            if (user == null)
                throw new NotFoundException("User was not found.");

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x =>
                    x.Id == request.SubscriptionPlanId &&
                    x.IsActive);

            if (plan == null)
                throw new NotFoundException("Subscription plan was not found.");

            if (plan.Price <= 0)
                throw new UserException("The Basic plan is not paid.");

            if (plan.DurationDays <= 0)
                throw new UserException("The selected plan is not valid for payment.");

            var hasActiveSubscription = await _context.Subscriptions
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.IsActive &&
                    x.SubscriptionPlanId == request.SubscriptionPlanId &&
                    (x.ExpiryDate == null || x.ExpiryDate > DateTime.UtcNow));

            if (hasActiveSubscription)
                throw new UserException("You already have an active subscription.");

            var alreadyProcessingPayment = await _context.Payments
                .Include(x => x.Subscription)
                .AnyAsync(x =>
                    x.Subscription != null &&
                    x.Subscription.UserId == request.UserId &&
                    x.PaymentStatus == "Processing");

            if (alreadyProcessingPayment)
                throw new UserException("You already have a payment in progress.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var subscription = new Subscription
                {
                    UserId = request.UserId,
                    SubscriptionPlanId = plan.Id,
                    StartDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(plan.DurationDays),
                    IsActive = false
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                var payment = new Payment
                {
                    SubscriptionId = subscription.Id,
                    PaymentStatus = "Pending",
                    CreatedAt = DateTime.UtcNow,
                    PaymentMethod = "Stripe",
                    PaymentAmount = plan.Price,
                    PaymentDate = null
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                var metadata = new Dictionary<string, string>
                {
                    ["paymentId"] = payment.Id.ToString(),
                    ["userId"] = user.Id.ToString(),
                    ["subscriptionId"] = subscription.Id.ToString(),
                    ["subscriptionPlanId"] = plan.Id.ToString()
                };

                var intent = await _stripeService.CreatePaymentIntentAsync(
                    (decimal)plan.Price,
                    _config.PaymentCurrency,
                    metadata
                );

                await _paymentState.GetState(payment.PaymentStatus).ToProcessingAsync(payment, intent.Id);
                await transaction.CommitAsync();

                return new
                {
                    clientSecret = intent.ClientSecret,
                    intentId = intent.Id,
                    paymentId = payment.Id,
                    subscriptionId = subscription.Id,
                    amount = payment.PaymentAmount,
                    currency = _config.PaymentCurrency,
                    planName = plan.Name
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task HandlePaymentIntentSucceededAsync(
            string paymentIntentId,
            IDictionary<string, string> metadata)
        {
            var payment = await GetPaymentFromMetadataAsync(paymentIntentId, metadata);

            if (payment == null)
                return;

            if (payment.PaymentStatus == "Paid")
                return;

            await _paymentState.GetState(payment.PaymentStatus).ToPaidAsync(payment);
        }

        public async Task HandlePaymentIntentFailedAsync(
            string paymentIntentId,
            IDictionary<string, string> metadata)
        {
            var payment = await GetPaymentFromMetadataAsync(paymentIntentId, metadata);

            if (payment == null)
                return;

            await _paymentState.GetState(payment.PaymentStatus).ToFailedAsync(payment);
        }

        public async Task HandlePaymentIntentCanceledAsync(
            string paymentIntentId,
            IDictionary<string, string> metadata)
        {
            var payment = await GetPaymentFromMetadataAsync(paymentIntentId, metadata);

            if (payment == null)
                return;

            await _paymentState.GetState(payment.PaymentStatus).ToCanceledAsync(payment);
        }

        private async Task<Payment?> GetPaymentFromMetadataAsync(
            string paymentIntentId,
            IDictionary<string, string> metadata)
        {
            if (!metadata.TryGetValue("paymentId", out var paymentIdString))
                return null;

            if (!int.TryParse(paymentIdString, out var paymentId))
                return null;

            var payment = await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == paymentId);

            if (payment == null)
                return null;

            payment.StripePaymentIntentId = paymentIntentId;

            return payment;
        }
    }
}