using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_Project_Backend.Models
{
    public class AchievementCriterion
    {
        [Key]
        public Guid CriterionId { get; set; }

        [Required]
        public Guid AchievementId { get; set; }

        // e.g. "DistinctAnimals", "EnvironmentComplete", "UnlockedInfoForAnimal", "TotalUnlockedInfo"
        [Required]
        public string CriterionType { get; set; } = null!;

        // integer threshold (e.g. required count)
        public int RequiredCount { get; set; }

        // optional target parameter (e.g. environment name or specific animal id as string)
        public string? Target { get; set; }
    }
}