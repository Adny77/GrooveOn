using GrooveOn.Services.Database;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class FailedPaymentState : BasePaymentState
    {
        public FailedPaymentState(
            GrooveOnDbContext context,
            IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
        }

        public override async Task ToProcessingAsync(int paymentId)
        {
            var payment = await GetPaymentAsync(paymentId);

            payment.PaymentStatus = "Processing";
            payment.FailureReason = null;

            await _context.SaveChangesAsync();
        }

        public override List<string> AllowedActions()
        {
            return new List<string>
    {
        nameof(ToProcessingAsync)
    };
        }
    }
}