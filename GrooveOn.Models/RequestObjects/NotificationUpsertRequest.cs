using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class NotificationUpsertRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = "system";

        public bool IsRead { get; set; }
    }
}
