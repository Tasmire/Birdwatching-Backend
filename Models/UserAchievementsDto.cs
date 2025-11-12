namespace Final_Project_Backend.Models
{
    public class UserAchievementsDto
    {
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public DateTime DateAchieved { get; set; }
    }

    public class UserAchievementsUpdateDto
    {
        public DateTime? DateAchieved { get; set; }
    }
}
