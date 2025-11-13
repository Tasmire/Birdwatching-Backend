namespace Final_Project_Backend.Models
{
    public class DashboardStatsViewModel
    {
        public int RegisteredPlayers { get; set; }
        public int AnimalCount { get; set; }
        public int TotalAchievements { get; set; }
        public long TotalUnlockedAchievements { get; set; }
        public double AverageAchievementProgressPercent { get; set; }
        public IEnumerable<(string EnvironmentName, int AnimalCount)> AnimalsPerEnvironment { get; set; } = Enumerable.Empty<(string, int)>();
        public DateTime? LatestUserRegisteredAt { get; set; } // optional: if you store timestamp
    }
}