using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class Environments
    {
        [Key]
        public Guid EnvironmentId { get; set; }
        public string Name { get; set; }

        [Display(Name = "Navigation Icon")]
        public string NavigationIcon { get; set; }

        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; }
    }
}
