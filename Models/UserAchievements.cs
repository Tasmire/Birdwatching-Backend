using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class UserAchievements
    {
        [Key]
        public Guid UserAchievementId { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; }
        public Guid AchievementId { get; set; }
        public Achievements Achievement { get; set; }
        public DateTime DateAchieved { get; set; }
    }
}
