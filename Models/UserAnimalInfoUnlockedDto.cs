namespace Final_Project_Backend.Models
{
    public class UserAnimalInfoUnlockedDto
    {
        public Guid UserId { get; set; }

        public Guid AnimalId { get; set; }
        public string? InfoType { get; set; }

        public bool IsUnlocked { get; set; } = true;
    }
}
