using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Final_Project_Backend.Models;

namespace Final_Project_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Final_Project_Backend.Models.Users> Users { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.Animals> Animals { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.Environments> Environments { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.Achievements> Achievements { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.SpawnLocations> SpawnLocations { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAnimals> UserAnimals { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAchievements> UserAchievements { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAnimalInfoUnlocked> UserAnimalInfoUnlocked { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAnimalPhotos> UserAnimalPhotos { get; set; } = default!;
    }
}
