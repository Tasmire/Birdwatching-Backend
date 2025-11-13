using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_Project_Backend.Data;
using Final_Project_Backend.Models;

namespace Final_Project_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAnimalInfoUnlockedAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserAnimalInfoUnlockedAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserAnimalInfoUnlockedAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAnimalInfoUnlocked>>> GetUserAnimalInfoUnlocked([FromQuery] Guid? userId, [FromQuery] Guid? animalId)
        {
            var q = _context.UserAnimalInfoUnlocked.AsQueryable();
            if (userId.HasValue) q = q.Where(u => u.UserId == userId.Value);
            if (animalId.HasValue) q = q.Where(u => u.AnimalId == animalId.Value);
            return await q.ToListAsync();
        }

        // GET: api/UserAnimalInfoUnlockedAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAnimalInfoUnlocked>> GetUserAnimalInfoUnlocked(Guid id)
        {
            var userAnimalInfoUnlocked = await _context.UserAnimalInfoUnlocked.FindAsync(id);

            if (userAnimalInfoUnlocked == null)
            {
                return NotFound();
            }

            return userAnimalInfoUnlocked;
        }

        // PUT: api/UserAnimalInfoUnlockedAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAnimalInfoUnlocked(Guid id, UserAnimalInfoUnlocked userAnimalInfoUnlocked)
        {
            if (id != userAnimalInfoUnlocked.UnlockId)
            {
                return BadRequest();
            }

            _context.Entry(userAnimalInfoUnlocked).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAnimalInfoUnlockedExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserAnimalInfoUnlockedAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAnimalInfoUnlocked>> PostUserAnimalInfoUnlocked([FromBody] UserAnimalInfoUnlockedDto dto)
        {
            if (dto == null) return BadRequest();

            // normalize incoming info type to canonical key
            string NormalizeInfoKeyLocal(string raw)
            {
                if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
                var k = System.Text.RegularExpressions.Regex.Replace(raw, "[^a-z0-9]", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase).ToLowerInvariant();
                if (k.Contains("maori")) return "maoriName";
                if (k.Contains("scientific")) return "scientificName";
                if (k.Contains("average") || k.Contains("size")) return "averageSize";
                if (k.Contains("habitat")) return "habitat";
                if (k.Contains("diet")) return "diet";
                if (k.Contains("origin")) return "origin";
                if (k.Contains("image")) return "imageUrl";
                return k; // fallback
            }

            var normalized = NormalizeInfoKeyLocal(dto.InfoType ?? "");

            // try to find existing record using the normalized key
            var existing = await _context.UserAnimalInfoUnlocked
                .Where(u => u.UserId == dto.UserId && u.AnimalId == dto.AnimalId && (u.InfoType ?? "").ToLower() == normalized.ToLower())
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                existing.IsUnlocked = dto.IsUnlocked;
                existing.InfoType = normalized;
                _context.Entry(existing).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(existing);
            }

            var entity = new UserAnimalInfoUnlocked
            {
                UnlockId = Guid.NewGuid(),
                UserId = dto.UserId,
                AnimalId = dto.AnimalId,
                InfoType = normalized,
                IsUnlocked = dto.IsUnlocked,
            };

            _context.UserAnimalInfoUnlocked.Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserAnimalInfoUnlocked), new { id = entity.UnlockId }, entity);
        }

        // DELETE: api/UserAnimalInfoUnlockedAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAnimalInfoUnlocked(Guid id)
        {
            var userAnimalInfoUnlocked = await _context.UserAnimalInfoUnlocked.FindAsync(id);
            if (userAnimalInfoUnlocked == null)
            {
                return NotFound();
            }

            _context.UserAnimalInfoUnlocked.Remove(userAnimalInfoUnlocked);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAnimalInfoUnlockedExists(Guid id)
        {
            return _context.UserAnimalInfoUnlocked.Any(e => e.UnlockId == id);
        }
    }
}
