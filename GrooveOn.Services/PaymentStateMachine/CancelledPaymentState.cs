using GrooveOn.Services.Database;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class CancelledPaymentState : BasePaymentState
    {
        public CancelledPaymentState(
            GrooveOnDbContext context,
            IServiceProvider serviceProvider)
            : base(context, serviceProvider)
        {
        }

        public override List<string> AllowedActions()
        {
            return new List<string>();
        }
    }
}