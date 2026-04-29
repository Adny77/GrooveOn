using GrooveOn.Services.Database;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class PaidPaymentState : BasePaymentState
    {
        public PaidPaymentState(
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