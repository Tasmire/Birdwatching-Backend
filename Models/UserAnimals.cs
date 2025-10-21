using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class UserAnimals
    {
        [Key]
        public Guid UserAnimalId { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; }
        public Guid AnimalId { get; set; }
        public Animals Animal { get; set; }
        public int TimesSpotted { get; set; }
    }
}
