using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Final_Project_Backend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementEvaluationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AchievementEvaluationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // canonical info keys that the frontend treats as unlockable
        private static readonly string[] CanonicalInfoKeys = new[] {
            "name", "maoriName", "scientificName", "averageSize", "habitat", "diet", "origin", "imageUrl"
        };

        // Normalize incoming InfoType strings to a canonical key (e.g. "Maori name" -> "maoriName")
        private static string NormalizeInfoKey(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            var k = Regex.Replace(raw, "[^a-z0-9]", "", RegexOptions.IgnoreCase).ToLowerInvariant();
            if (k.Contains("maori")) return "maoriName";
            if (k.Contains("scientific")) return "scientificName";
            if (k.Contains("average") || k.Contains("size")) return "averageSize";
            if (k.Contains("habitat")) return "habitat";
            if (k.Contains("diet")) return "diet";
            if (k.Contains("origin")) return "origin";
            if (k.Contains("image")) return "imageUrl";
            if (k.Contains("name")) return "name"; // map general "name" to canonical "name"
            return k; // fallback raw normalized token
        }

        // fetch distinct normalized unlocked info keys for a user+animal
        private async Task<HashSet<string>> GetUnlockedKeysForUserAnimalAsync(Guid userId, Guid animalId)
        {
            var raw = await _context.UserAnimalInfoUnlocked
                .Where(u => u.UserId == userId && u.AnimalId == animalId && u.IsUnlocked)
                .Select(u => u.InfoType)
                .ToListAsync();
            return raw
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(NormalizeInfoKey)
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .ToHashSet();
        }

        // fetch distinct normalized unlocked info keys for a user across all animals
        private async Task<HashSet<(Guid AnimalId, string Key)>> GetAllUnlockedPairsForUserAsync(Guid userId)
        {
            var rows = await _context.UserAnimalInfoUnlocked
                .Where(u => u.UserId == userId && u.IsUnlocked)
                .Select(u => new { u.AnimalId, u.InfoType })
                .ToListAsync();
            return rows
                .Where(r => r.InfoType != null)
                .Select(r => (r.AnimalId, Key: NormalizeInfoKey(r.InfoType)))
                .Where(p => !string.IsNullOrWhiteSpace(p.Key))
                .ToHashSet();
        }

        // POST: api/AchievementEvaluation
        // Frontend should POST when a relevant event happens (question answered, bird spotted, info unlocked)
        [HttpPost("evaluate")]
        public async Task<ActionResult<IEnumerable<UserAchievements>>> Evaluate([FromBody] AchievementEventDto evt)
        {
            if (evt == null) return BadRequest();

            // Attempt to use authenticated claim as fallback when evt.UserId is empty
            var currentUserClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (evt.UserId == Guid.Empty && !string.IsNullOrWhiteSpace(currentUserClaim) && Guid.TryParse(currentUserClaim, out var parsedClaimGuid))
            {
                evt.UserId = parsedClaimGuid;
            }

            Debug.WriteLine($"[Eval] incoming evt: UserId={evt.UserId} AnimalId={evt.AnimalId} EventType={evt.EventType} currentUserClaim={currentUserClaim}");

            var awarded = new List<UserAchievements>();

            // load user's already-awarded achievement ids
            var existing = await _context.UserAchievements
                .Where(ua => ua.UserId == evt.UserId)
                .Select(ua => ua.AchievementId)
                .ToListAsync();

            // load all achievements
            var achievements = await _context.Achievements.ToListAsync();

            Debug.WriteLine($"[Eval] evaluating {achievements.Count} achievements for user {evt.UserId}, alreadyAwarded={existing.Count}");

            foreach (var ach in achievements)
            {
                if (existing.Contains(ach.AchievementId)) 
                {
                    Debug.WriteLine($"[Eval] skipping already-awarded achievement {ach.Title} ({ach.AchievementId})");
                    continue;
                }

                // first: check if DB has explicit criteria for this achievement
                var criteria = await _context.AchievementCriteria
                    .Where(c => c.AchievementId == ach.AchievementId)
                    .ToListAsync();

                bool meets = false;

                Debug.WriteLine($"[Eval] checking achievement {ach.Title} ({ach.AchievementId}) - criteria count: {criteria.Count}");

                if (criteria.Any())
                {
                    // all criteria must be satisfied (AND)
                    meets = true;
                    foreach (var c in criteria)
                    {
                        Debug.WriteLine($"[Eval]  criterion {c.CriterionId} type={c.CriterionType} requiredCount={c.RequiredCount} target={c.Target}");
                        switch (c.CriterionType)
                        {
                            case "DistinctAnimals":
                                var distinctCount = await _context.UserAnimals
                                    .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                    .Select(ua => ua.AnimalId)
                                    .Distinct()
                                    .CountAsync();
                                Debug.WriteLine($"[Eval]   DistinctAnimals => userDistinct={distinctCount} required={c.RequiredCount}");
                                if (distinctCount < c.RequiredCount) meets = false;
                                break;

                            case "EnvironmentComplete":
                                if (string.IsNullOrEmpty(c.Target)) { Debug.WriteLine("[Eval]   EnvironmentComplete missing target -> fail"); meets = false; break; }
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

                                Debug.WriteLine($"[Eval]   EnvironmentComplete => totalInEnv={totalInEnvironment} userFoundInEnv={userFoundInEnv}");
                                if (totalInEnvironment == 0 || userFoundInEnv < totalInEnvironment) meets = false;
                                break;

                            case "UnlockedInfoForAnimal":
                                if (evt.AnimalId == null && string.IsNullOrEmpty(c.Target)) { Debug.WriteLine("[Eval]   UnlockedInfoForAnimal missing AnimalId/Target -> fail"); meets = false; break; }
                                var targetAnimalId = evt.AnimalId ?? Guid.Parse(c.Target!);
                                var unlockedKeys = await GetUnlockedKeysForUserAnimalAsync(evt.UserId, targetAnimalId);
                                var unlockedCount = unlockedKeys.Count(k => CanonicalInfoKeys.Contains(k));
                                Debug.WriteLine($"[Eval]   UnlockedInfoForAnimal => target={targetAnimalId} unlockedCanonicalCount={unlockedCount} required={c.RequiredCount}");
                                if (unlockedCount < c.RequiredCount) meets = false;
                                break;

                            case "TotalUnlockedInfo":
                                var allPairs = await _context.UserAnimalInfoUnlocked
                                    .Where(u => u.UserId == evt.UserId && u.IsUnlocked)
                                    .Select(u => new { u.AnimalId, u.InfoType })
                                    .ToListAsync();
                                var distinctPairs = allPairs
                                    .Where(r => !string.IsNullOrWhiteSpace(r.InfoType))
                                    .Select(r => (r.AnimalId, Key: NormalizeInfoKey(r.InfoType)))
                                    .Where(p => CanonicalInfoKeys.Contains(p.Key))
                                    .Distinct()
                                    .Count();
                                Debug.WriteLine($"[Eval]   TotalUnlockedInfo => userDistinctPairs={distinctPairs} required={c.RequiredCount}");
                                if (distinctPairs < c.RequiredCount) meets = false;
                                break;

                            case "UnlockedAllInfoForAnimal":
                                {
                                    Guid? aId = evt.AnimalId;
                                    if (aId == null && !string.IsNullOrEmpty(c.Target))
                                    {
                                        if (Guid.TryParse(c.Target, out var parsed)) aId = parsed;
                                    }

                                    if (aId != null)
                                    {
                                        var totalInfo = await GetTotalInfoTypesForAnimalAsync(aId.Value);
                                        var unlockedForAnimal = await GetUnlockedKeysForUserAnimalAsync(evt.UserId, aId.Value);
                                        var unlockedCountLocal = unlockedForAnimal.Count(k => CanonicalInfoKeys.Contains(k));
                                        Debug.WriteLine($"[Eval]   UnlockedAllInfoForAnimal (specific) => target={aId} totalInfo={totalInfo} unlocked={unlockedCountLocal}");
                                        if (totalInfo == 0 || unlockedCountLocal < totalInfo) meets = false;
                                    }
                                    else
                                    {
                                        var animals = await _context.Animals.ToListAsync();
                                        bool anyFully = false;
                                        foreach (var animal in animals)
                                        {
                                            var totalInfo = CountInfoFieldsOnAnimal(animal);
                                            if (totalInfo == 0) continue;
                                            var unlocked = await GetUnlockedKeysForUserAnimalAsync(evt.UserId, animal.AnimalId);
                                            var unlockedCountLocal = unlocked.Count(k => CanonicalInfoKeys.Contains(k));
                                            Debug.WriteLine($"[Eval]   UnlockedAllInfoForAnimal (any) => animal={animal.AnimalId} totalInfo={totalInfo} unlocked={unlockedCountLocal}");
                                            if (unlockedCountLocal >= totalInfo) { anyFully = true; break; }
                                        }
                                        if (!anyFully) meets = false;
                                    }
                                }
                                break;

                            case "UnlockedAllInfoForAllAnimals":
                                {
                                    var animals = await _context.Animals.ToListAsync();
                                    var totalPossible = animals.Sum(a => CountInfoFieldsOnAnimal(a));
                                    var allRows = await _context.UserAnimalInfoUnlocked
                                        .Where(u => u.UserId == evt.UserId && u.IsUnlocked)
                                        .Select(u => new { u.AnimalId, u.InfoType })
                                        .ToListAsync();
                                    var userDistinct = allRows
                                        .Where(r => !string.IsNullOrWhiteSpace(r.InfoType))
                                        .Select(r => (r.AnimalId, Key: NormalizeInfoKey(r.InfoType)))
                                        .Where(p => CanonicalInfoKeys.Contains(p.Key))
                                        .Distinct()
                                        .Count();
                                    Debug.WriteLine($"[Eval]   UnlockedAllInfoForAllAnimals => totalPossible={totalPossible} userDistinct={userDistinct}");
                                    if (totalPossible == 0 || userDistinct < totalPossible) meets = false;
                                }
                                break;

                            default:
                                Debug.WriteLine($"[Eval]   unknown criterion type '{c.CriterionType}' -> failing this criterion");
                                meets = false;
                                break;
                        }

                        if (!meets)
                        {
                            Debug.WriteLine($"[Eval]  criterion failed for achievement {ach.Title} ({ach.AchievementId})");
                            break;
                        }
                    }
                }
                else
                {
                    // fallback: handle common, easy-to-evaluate achievements by title/id
                    switch (ach.Title)
                    {
                        case string t when t.Contains("Love at First Flight", StringComparison.OrdinalIgnoreCase):
                            var any = await _context.UserAnimals.AnyAsync(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0);
                            Debug.WriteLine($"[Eval]  fallback Love at First Flight => anySpotted={any}");
                            meets = any;
                            break;

                        case string t when t.Contains("Feathered Friends", StringComparison.OrdinalIgnoreCase):
                            var distinct5 = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Select(ua => ua.AnimalId)
                                .Distinct()
                                .CountAsync();
                            Debug.WriteLine($"[Eval]  fallback Feathered Friends => distinct={distinct5}");
                            meets = distinct5 >= 5;
                            break;

                        case string t when t.Contains("Nature Enthusiast", StringComparison.OrdinalIgnoreCase):
                            var distinct15 = await _context.UserAnimals
                                .Where(ua => ua.UserId == evt.UserId && ua.TimesSpotted > 0)
                                .Select(ua => ua.AnimalId)
                                .Distinct()
                                .CountAsync();
                            Debug.WriteLine($"[Eval]  fallback Nature Enthusiast => distinct={distinct15}");
                            meets = distinct15 >= 15;
                            break;

                        case string t when t.Contains("Backyard", StringComparison.OrdinalIgnoreCase) || t.Contains("Urban", StringComparison.OrdinalIgnoreCase):
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

                            Debug.WriteLine($"[Eval]  fallback Urban/Backyard => totalUrban={totalUrban}, userUrban={userUrban}");
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

                            Debug.WriteLine($"[Eval]  fallback Coast => totalCoast={totalCoast}, userCoast={userCoast}");
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

                            Debug.WriteLine($"[Eval]  fallback Forest => totalForest={totalForest}, userForest={userForest}");
                            if (totalForest > 0 && userForest >= totalForest) meets = true;
                            break;

                        case string t when t.Contains("Bird Specialist", StringComparison.OrdinalIgnoreCase):
                            {
                                var animals = await _context.Animals.ToListAsync();
                                bool anyFullyUnlocked = false;
                                foreach (var a in animals)
                                {
                                    var total = CountInfoFieldsOnAnimal(a);
                                    if (total == 0) continue;
                                    var unlocked = await GetUnlockedKeysForUserAnimalAsync(evt.UserId, a.AnimalId);
                                    var unlockedCount = unlocked.Count(k => CanonicalInfoKeys.Contains(k));
                                    Debug.WriteLine($"[Eval]  fallback Bird Specialist => animal={a.AnimalId} total={total} unlocked={unlockedCount}");
                                    if (unlockedCount >= total) { anyFullyUnlocked = true; break; }
                                }
                                meets = anyFullyUnlocked;
                            }
                            break;

                        case string t when t.Contains("Avian Expert", StringComparison.OrdinalIgnoreCase) || t.Contains("logbook", StringComparison.OrdinalIgnoreCase):
                            {
                                var animals = await _context.Animals.ToListAsync();
                                var totalPossible = animals.Sum(a => CountInfoFieldsOnAnimal(a));
                                var allRows = await _context.UserAnimalInfoUnlocked
                                    .Where(u => u.UserId == evt.UserId && u.IsUnlocked)
                                    .Select(u => new { u.AnimalId, u.InfoType })
                                    .ToListAsync();
                                var userUnlockedDistinct = allRows
                                    .Where(r => !string.IsNullOrWhiteSpace(r.InfoType))
                                    .Select(r => (r.AnimalId, Key: NormalizeInfoKey(r.InfoType)))
                                    .Where(p => CanonicalInfoKeys.Contains(p.Key))
                                    .Distinct()
                                    .Count();
                                Debug.WriteLine($"[Eval]  fallback Avian Expert => totalPossible={totalPossible}, userDistinct={userUnlockedDistinct}");
                                meets = totalPossible > 0 && userUnlockedDistinct >= totalPossible;
                            }
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
                    Debug.WriteLine($"[Eval] will award achievement {ach.Title} ({ach.AchievementId}) to user {evt.UserId}");
                    awarded.Add(ua);
                }
                else
                {
                    Debug.WriteLine($"[Eval] achievement not met: {ach.Title} ({ach.AchievementId}) for user {evt.UserId}");
                }
            }

            if (awarded.Any())
            {
                await _context.SaveChangesAsync();
                Debug.WriteLine($"[Eval] saved {awarded.Count} new UserAchievements for user {evt.UserId}");
            }
            else
            {
                Debug.WriteLine($"[Eval] no achievements to save for user {evt.UserId}");
            }

            return Ok(awarded);
        }

        // Helper: count how many info fields we consider "unlockable" for an animal.
        // This implementation derives the count from non-null properties on the Animals model.
        // If you maintain a canonical list/table of info types per animal, replace this logic to read that.
        private int CountInfoFieldsOnAnimal(Animals a)
        {
            // Only count the fields the frontend treats as unlockable (match logbook.js infoFields)
            var count = 0;
            if (!string.IsNullOrWhiteSpace(a.Name)) count++;            // count common name
            if (!string.IsNullOrWhiteSpace(a.MaoriName)) count++;
            if (!string.IsNullOrWhiteSpace(a.ScientificName)) count++;
            if (!string.IsNullOrWhiteSpace(a.AverageSize)) count++;
            if (!string.IsNullOrWhiteSpace(a.Habitat)) count++;
            if (!string.IsNullOrWhiteSpace(a.Diet)) count++;
            if (!string.IsNullOrWhiteSpace(a.Origin)) count++;
            if (!string.IsNullOrWhiteSpace(a.ImageUrl)) count++;        // optional image unlock
            return count;
        }

        private async Task<int> GetTotalInfoTypesForAnimalAsync(Guid animalId)
        {
            var animal = await _context.Animals.FindAsync(animalId);
            if (animal == null) return 0;
            return CountInfoFieldsOnAnimal(animal);
        }
    }
}