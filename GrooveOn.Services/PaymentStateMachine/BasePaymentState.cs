using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace GrooveOn.Services.PaymentStateMachine
{
    public class BasePaymentState(
        IServiceProvider serviceProvider,
        GrooveOnDbContext context,
        IMapper mapper)
    {
        protected readonly IServiceProvider _serviceProvider = serviceProvider;
        protected readonly GrooveOnDbContext _context = context;
        protected readonly IMapper _mapper = mapper;

        public virtual async Task ToProcessingAsync(Payment payment, string stripeIntentId)
        {
            throw new UserException("Transition to 'Processing' is not allowed from this state.");
        }

        public virtual async Task ToPaidAsync(Payment payment)
        {
            throw new UserException("Transition to 'Paid' is not allowed from this state.");
        }

        public virtual async Task ToFailedAsync(Payment payment)
        {
            throw new UserException("Transition to 'Failed' is not allowed from this state.");
        }

        public virtual async Task ToCanceledAsync(Payment payment)
        {
            throw new UserException("Transition to 'Canceled' is not allowed from this state.");
        }

        public virtual List<string> AllowedActions() => [];

        public BasePaymentState GetState(string status)
        {
            return status switch
            {
                "Pending" => _serviceProvider.GetRequiredService<PendingPaymentState>(),
                "Processing" => _serviceProvider.GetRequiredService<ProcessingPaymentState>(),
                "Paid" => _serviceProvider.GetRequiredService<PaidPaymentState>(),
                "Failed" => _serviceProvider.GetRequiredService<FailedPaymentState>(),
                "Canceled" => _serviceProvider.GetRequiredService<CancelledPaymentState>(),
                _ => throw new InvalidOperationException($"Unknown payment status: '{status}'.")
            };
        }
    }
}
