using GrooveOn.Services.Database;
using MapsterMapper;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class PaidPaymentState(
        IServiceProvider serviceProvider,
        GrooveOnDbContext context,
        IMapper mapper) : BasePaymentState(serviceProvider, context, mapper)
    {
    }
}
