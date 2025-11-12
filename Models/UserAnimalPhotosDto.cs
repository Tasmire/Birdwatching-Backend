namespace Final_Project_Backend.Models
{
    public class UserAnimalPhotosDto
    {
        public Guid UserId { get; set; }
        public Guid AnimalId { get; set; }
        public string PhotoUrl { get; set; } = null!;
        public DateTime DateUploaded { get; set; }
    }
}
