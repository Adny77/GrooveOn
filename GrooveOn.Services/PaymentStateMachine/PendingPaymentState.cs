using GrooveOn.Services.Database;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class PendingPaymentState : BasePaymentState
    {
        public PendingPaymentState(
            GrooveOnDbContext context,
            IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
        }

        public override async Task ToProcessingAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            payment.PaymentStatus = "Processing";

            await _context.SaveChangesAsync();
        }

        public override async Task ToCancelledAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            payment.PaymentStatus = "Canceled";

            await _context.SaveChangesAsync();
        }

        public override List<string> AllowedActions()
        {
            return new List<string>
    {
        nameof(ToProcessingAsync),
        nameof(ToCancelledAsync)
    };
        }
    }
}