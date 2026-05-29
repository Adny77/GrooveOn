using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        public User? User { get; set; }

        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Type { get; set; } = "system";

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
