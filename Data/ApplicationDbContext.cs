using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Final_Project_Backend.Models;

namespace Final_Project_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Final_Project_Backend.Models.Animals> Animals { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.Environments> Environments { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.Achievements> Achievements { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.SpawnLocations> SpawnLocations { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAnimals> UserAnimals { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAchievements> UserAchievements { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAnimalInfoUnlocked> UserAnimalInfoUnlocked { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.UserAnimalPhotos> UserAnimalPhotos { get; set; } = default!;
        public DbSet<Final_Project_Backend.Models.AchievementCriterion> AchievementCriteria { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Users entity to existing Users table and map each Identity property to your column names
            modelBuilder.Entity<Users>(b =>
            {
                b.ToTable("Users"); // existing table name

                b.HasKey(u => u.Id);
                b.Property(u => u.Id).HasColumnName("UserId");
                b.Property(u => u.UserName).HasColumnName("Username");
                b.Property(u => u.NormalizedUserName).HasColumnName("NormalizedUsername");
                b.Property(u => u.Email).HasColumnName("Email");
                b.Property(u => u.NormalizedEmail).HasColumnName("NormalizedEmail");
                b.Property(u => u.PasswordHash).HasColumnName("PasswordHash");
                b.Property(u => u.SecurityStamp).HasColumnName("SecurityStamp");
                b.Property(u => u.ConcurrencyStamp).HasColumnName("ConcurrencyStamp");
                b.Property(u => u.PhoneNumber).HasColumnName("PhoneNumber");
                b.Property(u => u.PhoneNumberConfirmed).HasColumnName("PhoneNumberConfirmed");
                b.Property(u => u.EmailConfirmed).HasColumnName("EmailConfirmed");
                b.Property(u => u.LockoutEnabled).HasColumnName("LockoutEnabled");
                b.Property(u => u.LockoutEnd).HasColumnName("LockoutEnd");
                b.Property(u => u.AccessFailedCount).HasColumnName("AccessFailedCount");

                // map application-specific fields
                b.Property(u => u.DisplayName).HasColumnName("DisplayName");
            });

            // Optional: if your database uses different table names (for example "Roles", "UserRoles", etc.)
            // you can map the other Identity tables as well. Adjust names to match your DB or remove if not needed:
            modelBuilder.Entity<IdentityRole<Guid>>(b => b.ToTable("AspNetRoles"));
            modelBuilder.Entity<IdentityUserRole<Guid>>(b => b.ToTable("AspNetUserRoles"));
            modelBuilder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable("AspNetUserClaims"));
            modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable("AspNetRoleClaims"));
            modelBuilder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable("AspNetUserLogins"));
            modelBuilder.Entity<IdentityUserToken<Guid>>(b => b.ToTable("AspNetUserTokens"));

            // Keep existing relationships (example unchanged)
            modelBuilder.Entity<Animals>()
                .HasMany(a => a.SpawnLocations)
                .WithMany(s => s.Animals)
                .UsingEntity<Dictionary<string, object>>(
                    "AnimalSpawnLocations",
                    j => j.HasOne<SpawnLocations>().WithMany().HasForeignKey("SpawnLocationId"),
                    j => j.HasOne<Animals>().WithMany().HasForeignKey("AnimalId"),
                    j => j.HasKey("AnimalId", "SpawnLocationId")
                );
        }
    }
}
