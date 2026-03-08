using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }

        public string? PasswordSalt { get; set; }

        public string Email { get; set; } = string.Empty;

        public bool IsActive {get; set;}

        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}