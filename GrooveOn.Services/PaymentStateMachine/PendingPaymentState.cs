using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class PendingPaymentState(
        IServiceProvider serviceProvider,
        GrooveOnDbContext context,
        IMapper mapper) : BasePaymentState(serviceProvider, context, mapper)
    {
        public override async Task ToProcessingAsync(Payment payment, string stripeIntentId)
        {
            payment.StripePaymentIntentId = stripeIntentId;
            payment.PaymentStatus = "Processing";
            await _context.SaveChangesAsync();
        }

        public override async Task ToCanceledAsync(Payment payment)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == payment.SubscriptionId);

            if (subscription != null)
            {
                subscription.IsActive = false;

                _serviceProvider.GetRequiredService<INotificationService>().AddForUser(
                    subscription.UserId,
                    "Payment Canceled",
                    "Your payment has been canceled and the subscription was not activated.",
                    "payment_canceled");
            }

            payment.PaymentStatus = "Canceled";
            payment.FailureReason = "Payment was canceled.";
            await _context.SaveChangesAsync();
        }

        public override List<string> AllowedActions() =>
            [nameof(ToProcessingAsync), nameof(ToCanceledAsync)];
    }
}
