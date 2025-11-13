using System;

namespace Final_Project_Backend.Models
{
    public class AchievementEventDto
    {
        public Guid UserId { get; set; }

        // event type from frontend, e.g. "QuestionAnswered", "BirdSpotted", "InfoUnlocked"
        public string EventType { get; set; } = null!;

        // optional animal id for events that involve a bird
        public Guid? AnimalId { get; set; }

        // optional info type for info-unlock events
        public string? InfoType { get; set; }
    }
}