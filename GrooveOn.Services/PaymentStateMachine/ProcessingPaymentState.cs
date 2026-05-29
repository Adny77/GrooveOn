using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class ProcessingPaymentState(
        IServiceProvider serviceProvider,
        GrooveOnDbContext context,
        IMapper mapper) : BasePaymentState(serviceProvider, context, mapper)
    {
        public override async Task ToPaidAsync(Payment payment)
        {
            var subscription = await _context.Subscriptions
                .Include(x => x.SubscriptionPlan)
                .FirstOrDefaultAsync(x => x.Id == payment.SubscriptionId);

            if (subscription != null)
            {
                var oldSubs = await _context.Subscriptions
                    .Where(x =>
                        x.UserId == subscription.UserId &&
                        x.IsActive &&
                        x.Id != subscription.Id)
                    .ToListAsync();

                foreach (var s in oldSubs)
                    s.IsActive = false;

                subscription.IsActive = true;
                subscription.StartDate = DateTime.UtcNow;
                subscription.ExpiryDate = DateTime.UtcNow.AddDays(
                    subscription.SubscriptionPlan?.DurationDays ?? 30);

                var planName = subscription.SubscriptionPlan?.Name ?? "selected";
                var expiryText = subscription.ExpiryDate?.ToString("dd.MM.yyyy");

                _serviceProvider.GetRequiredService<INotificationService>().AddForUser(
                    subscription.UserId,
                    "Članarina je aktivirana",
                    $"Uplata za {planName} plan je uspješna. Vaša članarina vrijedi do {expiryText}.",
                    "subscription_paid");
            }

            payment.PaymentStatus = "Paid";
            payment.PaidAt = DateTime.UtcNow;
            payment.PaymentDate = DateTime.UtcNow;
            payment.FailureReason = null;
            payment.PaymentMethod = "Stripe";

            await _context.SaveChangesAsync();
        }

        public override async Task ToFailedAsync(Payment payment)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == payment.SubscriptionId);

            if (subscription != null)
                subscription.IsActive = false;

            if (subscription != null)
            {
                _serviceProvider.GetRequiredService<INotificationService>().AddForUser(
                    subscription.UserId,
                    "Uplata nije uspjela",
                    "Vaša uplata nije mogla biti završena. Pokušajte ponovo ili koristite drugi način plaćanja.",
                    "payment_failed");
            }

            payment.PaymentStatus = "Failed";
            payment.PaidAt = null;
            payment.PaymentDate = null;
            payment.FailureReason = "Payment was not completed successfully.";
            payment.PaymentMethod = "Stripe";

            await _context.SaveChangesAsync();
        }

        public override async Task ToCanceledAsync(Payment payment)
        {
            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Id == payment.SubscriptionId);

            if (subscription != null)
                subscription.IsActive = false;

            if (subscription != null)
            {
                _serviceProvider.GetRequiredService<INotificationService>().AddForUser(
                    subscription.UserId,
                    "Uplata otkazana",
                    "Vaša uplata je otkazana i članarina nije aktivirana.",
                    "payment_canceled");
            }

            payment.PaymentStatus = "Canceled";
            payment.PaidAt = null;
            payment.PaymentDate = null;
            payment.FailureReason = "Payment was canceled.";
            payment.PaymentMethod = "Stripe";

            await _context.SaveChangesAsync();
        }

        public override List<string> AllowedActions() =>
            [nameof(ToPaidAsync), nameof(ToFailedAsync), nameof(ToCanceledAsync)];
    }
}
