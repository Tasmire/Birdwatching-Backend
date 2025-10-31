using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class Animals
    {
        [Key]
        public Guid AnimalId { get; set; }
        public string Name { get; set; } = null!;

        [Display(Name = "Maori Name")]
        public string? MaoriName { get; set; }

        [Display(Name = "Scientific Name")]
        public string? ScientificName { get; set; }

        [Display(Name = "Average Size")]
        public string? AverageSize { get; set; }
        public string? Habitat { get; set; }
        public string? Diet { get; set; }
        public string? Origin { get; set; }

        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        public Guid EnvironmentId { get; set; }
        public Environments? Environment { get; set; }
        public ICollection<SpawnLocations> SpawnLocations { get; set; } = new List<SpawnLocations>();
    }
}
