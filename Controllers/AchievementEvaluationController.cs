using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final_Project_Backend.Controllers
{
    [Authorize(Roles = "Staff, Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementEvaluationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AchievementEvaluationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/AchievementEvaluation
        // Frontend should POST when a relevant event happens (question answered, bird spotted, info unlocked)
        [HttpPost("evaluate")]
        public async Task<ActionResult<IEnumerable<UserAchievements>>> Evaluate([FromBody] AchievementEventDto evt)
        {
            if (evt == null) return BadRequest();

            var awarded = new List<UserAchievements>();

            // load user's already-awarded achievement ids
            var existing = await _context.UserAchievements
                .Where(ua => ua.UserId == evt.UserId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            // load all achievements
            var achievements = await _context.Achievements.ToListAsync();

            foreach (var ach in achievements)
            {
                if (existing.Contains(ach.AchievementId)) continue;

                // first: check if DB has explicit criteria for this achievement
                var criteria = await _context.AchievementCriteria
                    .Where(c => c.AchievementId == ach.AchievementId)
                    .ToListAsync();

                bool meets = false;

                if (criteria.Any())
                {
                    // all criteria must be satisfied (AND)
                    meets = true;
                    foreach (var c in criteria)
                    {
                        switch (c.CriterionType)
                        {
                            case "DistinctAnimals":
                                var distinctCount = await _context.UserAnimals
                                    .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                    .Select(ua => ua.AnimalId)
                                    .Distinct()
                                    .CountAsync();
                                if (distinctCount < c.RequiredCount) meets = false;
                                break;

                            case "EnvironmentComplete":
                                // Target = environment name (case-insensitive contains)
                                if (string.IsNullOrEmpty(c.Target)) { meets = false; break; }
                                var envName = c.Target.ToLower();
                                var totalInEnvironment = await _context.Animals
                                    .Include(a => a.Environment)
                                    .Where(a => a.Environment != null && a.Environment.Name.ToLower().Contains(envName))
                                    .Select(a => a.AnimalId)
                                    .Distinct()
                                    .CountAsync();

                                var userFoundInEnv = await _context.UserAnimals
                                    .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                    .Join(_context.Animals,
                                          ua => ua.AnimalId,
                                          a => a.AnimalId,
                                          (ua, a) => new { a })
                                    .Where(x => x.a.Environment != null && x.a.Environment.Name.ToLower().Contains(envName))
                                    .Select(x => x.a.AnimalId)
                                    .Distinct()
                                    .CountAsync();

                                if (totalInEnvironment == 0 || userFoundInEnv < totalInEnvironment) meets = false;
                                break;

                            case "UnlockedInfoForAnimal":
                                if (!evt.AnimalId.HasValue) { meets = false; break; }
                                var unlockedForAnimal = await _context.UserAnimalInfoUnlocked
                                    .Where(u => u.UserId == evt.UserId && u.AnimalId == evt.AnimalId && u.IsUnlocked)
                                    .CountAsync();
                                if (unlockedForAnimal < c.RequiredCount) meets = false;
                                break;

                            case "TotalUnlockedInfo":
                                var totalUnlocked = await _context.UserAnimalInfoUnlocked
                                    .Where(u => u.UserId == evt.UserId && u.IsUnlocked)
                                    .CountAsync();
                                if (totalUnlocked < c.RequiredCount) meets = false;
                                break;

                            default:
                                meets = false;
                                break;
                        }

                        if (!meets) break;
                    }
                }
                else
                {
                    // fallback: handle common, easy-to-evaluate achievements by title/id
                    // (You can remove fallback and rely entirely on DB criteria.)
                    switch (ach.Title)
                    {
                        case string t when t.Contains("Love at First Flight", StringComparison.OrdinalIgnoreCase):
                            var any = await _context.UserAnimals.AnyAsync(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0);
                            meets = any;
                            break;

                        case string t when t.Contains("Feathered Friends", StringComparison.OrdinalIgnoreCase):
                            var distinct5 = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Select(ua => ua.AnimalId)
                                .Distinct()
                                .CountAsync();
                            meets = distinct5 >= 5;
                            break;

                        case string t when t.Contains("Nature Enthusiast", StringComparison.OrdinalIgnoreCase):
                            var distinct15 = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Select(ua => ua.AnimalId)
                                .Distinct()
                                .CountAsync();
                            meets = distinct15 >= 15;
                            break;

                        case string t when t.Contains("Backyard", StringComparison.OrdinalIgnoreCase) || t.Contains("Urban", StringComparison.OrdinalIgnoreCase):
                            // Try to detect environment name containing "urban" or "backyard"
                            var totalUrban = await _context.Animals
                                .Include(a => a.Environment)
                                .Where(a => a.Environment != null && a.Environment.Name.ToLower().Contains("urban"))
                                .Select(a => a.AnimalId)
                                .Distinct()
                                .CountAsync();

                            var userUrban = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Join(_context.Animals, ua => ua.AnimalId, a => a.AnimalId, (ua, a) => a)
                                .Where(a => a.Environment != null && a.Environment.Name.ToLower().Contains("urban"))
                                .Select(a => a.AnimalId)
                                .Distinct()
                                .CountAsync();

                            if (totalUrban > 0 && userUrban >= totalUrban) meets = true;
                            break;

                        case string t when t.Contains("Coast", StringComparison.OrdinalIgnoreCase) || t.Contains("Coastline", StringComparison.OrdinalIgnoreCase):
                            var totalCoast = await _context.Animals
                                .Include(a => a.Environment)
                                .Where(a => a.Environment != null && (a.Environment.Name.ToLower().Contains("coast") || a.Environment.Name.ToLower().Contains("coastal") || a.Environment.Name.ToLower().Contains("water")))
                                .Select(a => a.AnimalId).Distinct().CountAsync();

                            var userCoast = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Join(_context.Animals, ua => ua.AnimalId, a => a.AnimalId, (ua, a) => a)
                                .Where(a => a.Environment != null && (a.Environment.Name.ToLower().Contains("coast") || a.Environment.Name.ToLower().Contains("coastal") || a.Environment.Name.ToLower().Contains("water")))
                                .Select(a => a.AnimalId)
                                .Distinct()
                                .CountAsync();

                            if (totalCoast > 0 && userCoast >= totalCoast) meets = true;
                            break;

                        case string t when t.Contains("Woodland", StringComparison.OrdinalIgnoreCase) || t.Contains("Forest", StringComparison.OrdinalIgnoreCase) || t.Contains("Wood", StringComparison.OrdinalIgnoreCase):
                            var totalForest = await _context.Animals
                                .Include(a => a.Environment)
                                .Where(a => a.Environment != null && (a.Environment.Name.ToLower().Contains("forest") || a.Environment.Name.ToLower().Contains("wood") || a.Environment.Name.ToLower().Contains("tree")))
                                .Select(a => a.AnimalId).Distinct().CountAsync();

                            var userForest = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Join(_context.Animals, ua => ua.AnimalId, a => a.AnimalId, (ua, a) => a)
                                .Where(a => a.Environment != null && (a.Environment.Name.ToLower().Contains("forest") || a.Environment.Name.ToLower().Contains("wood") || a.Environment.Name.ToLower().Contains("tree")))
                                .Select(a => a.AnimalId)
                                .Distinct()
                                .CountAsync();

                            if (totalForest > 0 && userForest >= totalForest) meets = true;
                            break;

                        default:
                            meets = false;
                            break;
                    }
                }

                if (meets)
                {
                    var ua = new UserAchievements
                    {
                        UserAchievementId = Guid.NewGuid(),
                        UserId = evt.UserId,
                        AchievementId = ach.AchievementId,
                        DateAchieved = DateTime.UtcNow
                    };
                    _context.UserAchievements.Add(ua);
                    awarded.Add(ua);
                }
            }

            if (awarded.Any())
            {
                await _context.SaveChangesAsync();
            }

            return Ok(awarded);
        }
    }
}