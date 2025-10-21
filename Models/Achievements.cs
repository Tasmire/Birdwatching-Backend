using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class Achievements
    {
        [Key]
        public Guid AchievementId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string IconUrl { get; set; } = null!;
    }
}
