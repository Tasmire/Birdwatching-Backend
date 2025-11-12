namespace Final_Project_Backend.Models
{
    public class UserAnimalsDto
    {
        public Guid UserId { get; set; }
        public Guid AnimalId { get; set; }
        public int TimesSpotted { get; set; }
    }

    public class UserAnimalsUpdateDto
    {
        public int TimesSpotted { get; set; }
    }
}
