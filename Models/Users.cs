using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class Users
    {
        [Key]
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
