using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class Environments
    {
        [Key]
        public Guid EnvironmentId { get; set; }
        public string Name { get; set; }
        public string NavigationIcon { get; set; }
        public string ImageUrl { get; set; }
    }
}
