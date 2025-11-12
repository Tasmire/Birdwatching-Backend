using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class SpawnLocations
    {
        [Key]
        public Guid SpawnLocationId { get; set; }
        public string Name { get; set; } = null!;

        [Display(Name = "Spawn Type")]
        public string SpawnType { get; set; } = null!;

        [Display(Name = "X Coordinate")]
        public float XCoordinate { get; set; }

        [Display(Name = "Y Coordinate")]
        public float YCoordinate { get; set; }
        public float Scale { get; set; }
        public Guid AnimalId { get; set; }
        public Animals? Animal { get; set; } 
    }
}
