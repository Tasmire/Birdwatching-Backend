using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class Animals
    {
        [Key]
        public Guid AnimalId { get; set; }
        public string Name { get; set; } = null!;
        public string MaoriName { get; set; }
        public string ScientificName { get; set; }
        public string AverageSize { get; set; }
        public string Habitat { get; set; }
        public string Diet { get; set; }
        public string Origin { get; set; }
        public string ImageUrl { get; set; }
        public Guid EnvironmentId { get; set; }
        public Environments Environment { get; set; }
        public ICollection<SpawnLocations> SpawnLocations { get; set; } = new List<SpawnLocations>();
    }
}
