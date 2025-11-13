using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Final_Project_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final_Project_Backend.Data
{
    public static class AchievementCriteriaSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var achievements = await ctx.Achievements.ToListAsync();
            if (!achievements.Any()) return;

            Guid FindByTitleContains(string part) =>
                achievements.FirstOrDefault(a => a.Title != null && a.Title.Contains(part, StringComparison.OrdinalIgnoreCase))
                            ?.AchievementId ?? Guid.Empty;

            var toAdd = new List<AchievementCriterion>();

            // simple / distinct-animal based
            var id = FindByTitleContains("Love at First Flight");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "DistinctAnimals"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "DistinctAnimals", RequiredCount = 1 });

            id = FindByTitleContains("Feathered Friends");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "DistinctAnimals" && c.RequiredCount == 5))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "DistinctAnimals", RequiredCount = 5 });

            id = FindByTitleContains("Nature Enthusiast");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "DistinctAnimals" && c.RequiredCount == 15))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "DistinctAnimals", RequiredCount = 15 });

            // info-unlock based
            id = FindByTitleContains("Bird Specialist");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "UnlockedAllInfoForAnimal"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "UnlockedAllInfoForAnimal", RequiredCount = 0, Target = null }); // NULL target = any single animal fully unlocked

            id = FindByTitleContains("Avian Expert");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "UnlockedAllInfoForAllAnimals"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "UnlockedAllInfoForAllAnimals", RequiredCount = 0 });

            // environment-completion based (Backyard/Urban / Coast / Woodland)
            id = FindByTitleContains("Backyard");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "EnvironmentComplete"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "EnvironmentComplete", RequiredCount = 0, Target = "urban" });

            id = FindByTitleContains("Coast");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "EnvironmentComplete"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "EnvironmentComplete", RequiredCount = 0, Target = "coast" });

            id = FindByTitleContains("Woodland");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "EnvironmentComplete"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "EnvironmentComplete", RequiredCount = 0, Target = "forest" });

            // Optional: first-photo / photo-count achievements (controller must implement)
            id = FindByTitleContains("Photographer");
            if (id != Guid.Empty && !await ctx.AchievementCriteria.AnyAsync(c => c.AchievementId == id && c.CriterionType == "TotalPhotosUploaded"))
                toAdd.Add(new AchievementCriterion { CriterionId = Guid.NewGuid(), AchievementId = id, CriterionType = "TotalPhotosUploaded", RequiredCount = 10 });

            if (toAdd.Any())
            {
                ctx.AchievementCriteria.AddRange(toAdd);
                await ctx.SaveChangesAsync();
            }
        }
    }
}