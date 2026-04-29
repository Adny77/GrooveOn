using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrooveOn.Services.PaymentStateMachine
{
    public abstract class BasePaymentState
    {
        protected readonly GrooveOnDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        protected BasePaymentState(
            GrooveOnDbContext context,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public BasePaymentState GetState(string stateName)
        {
            var state = _serviceProvider.GetService(
                Type.GetType($"GrooveOn.Services.PaymentStateMachine.{stateName}")
            ) as BasePaymentState;

            if (state == null)
                throw new UserException($"Payment state '{stateName}' is not registered.");

            return state;
        }

        public virtual Task ToProcessingAsync(int paymentId)
            => throw new UserException("Nedozvoljena promjena statusa.");

        public virtual Task ToPaidAsync(int paymentId)
            => throw new UserException("Nedozvoljena promjena statusa.");

        public virtual Task ToFailedAsync(int paymentId)
            => throw new UserException("Nedozvoljena promjena statusa.");

        public virtual Task ToCancelledAsync(int paymentId)
            => throw new UserException("Nedozvoljena promjena statusa.");

        protected async Task<Payment> GetPaymentAsync(int paymentId)
        {
            var payment = await _context.Payments
                .Include(x => x.Subscription)
                    .ThenInclude(x => x.User)
                .Include(x => x.Subscription)
                    .ThenInclude(x => x.SubscriptionPlan)
                .FirstOrDefaultAsync(x => x.Id == paymentId);

            if (payment == null)
                throw new UserException("Payment was not found.");

            if (payment.Subscription == null)
                throw new UserException("Subscription was not found.");

            return payment;
        }

        public virtual List<string> AllowedActions()
        {
            return new List<string>();
        }
    }
}