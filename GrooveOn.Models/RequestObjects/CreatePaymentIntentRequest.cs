using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class CreatePaymentIntentRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }
    }
}