using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class SpawnLocations
    {
        [Key]
        public Guid SpawnLocationId { get; set; }
        public string Name { get; set; }
        public string SpawnType { get; set; } = null!;
        public float XCoordinate { get; set; }
        public float YCoordinate { get; set; }
        public float ZCoordinate { get; set; }
        public Guid AnimalId { get; set; }
        public Animals Animal { get; set; }
        public Guid EnvironmentId { get; set; }
    }
}
