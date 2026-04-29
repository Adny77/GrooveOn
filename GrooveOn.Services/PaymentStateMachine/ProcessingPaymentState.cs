using GrooveOn.Services.Database;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class ProcessingPaymentState : BasePaymentState
    {
        public ProcessingPaymentState(
            GrooveOnDbContext context,
            IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
        }

        public override async Task ToPaidAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.UtcNow;
            payment.PaymentDate = DateTime.UtcNow;
            payment.PaymentMethod = "Stripe";
            payment.FailureReason = null;

            if (payment.Subscription != null)
            {
                payment.Subscription.IsActive = true;
                payment.Subscription.StartDate = DateTime.UtcNow;

                var durationDays = payment.Subscription.SubscriptionPlan?.DurationDays ?? 30;
                payment.Subscription.ExpiryDate = DateTime.UtcNow.AddDays(durationDays);
            }

            await _context.SaveChangesAsync();
        }

        public override async Task ToFailedAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            payment.PaymentStatus = "Failed";
            payment.PaidAt = null;
            payment.PaymentDate = null;
            payment.FailureReason ??= "Stripe payment failed.";
            payment.PaymentMethod = "Stripe";

            if (payment.Subscription != null)
                payment.Subscription.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public override async Task ToCancelledAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            payment.PaymentStatus = "Canceled";
            payment.PaidAt = null;
            payment.PaymentDate = null;
            payment.FailureReason = "Payment was canceled.";
            payment.PaymentMethod = "Stripe";

            if (payment.Subscription != null)
                payment.Subscription.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public override List<string> AllowedActions()
        {
            return new List<string>
            {
                nameof(ToPaidAsync),
                nameof(ToFailedAsync),
                nameof(ToCancelledAsync)
            };
        }
    }
}