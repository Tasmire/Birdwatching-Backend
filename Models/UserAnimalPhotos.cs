using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class UserAnimalPhotos
    {
        [Key]
        public Guid UserAnimalPhotoId { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; }
        public Guid AnimalId { get; set; }
        public Animals Animal { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public DateTime DateUploaded { get; set; }
    }
}
