using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class UserAnimalInfoUnlocked
    {
        [Key]
        public Guid UnlockId { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; }
        public Guid AnimalId { get; set; }
        public UserAnimals Animal { get; set; }
        public string InfoType { get; set; } = null!;
        public bool IsUnlocked { get; set; }
    }
}
