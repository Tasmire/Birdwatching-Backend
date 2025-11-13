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
        public async Task<ActionResult<IEnumerable<UserAnimalInfoUnlocked>>> GetUserAnimalInfoUnlocked()
        {
            return await _context.UserAnimalInfoUnlocked.ToListAsync();
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
        public async Task<ActionResult<UserAnimalInfoUnlocked>> PostUserAnimalInfoUnlocked(UserAnimalInfoUnlockedDto dto)
        {
            if (dto == null) return BadRequest();

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest(new { errors = new { User = new[] { "User not found." } } });

            var animal = await _context.Animals.FindAsync(dto.AnimalId);
            if (animal == null) return BadRequest(new { errors = new { Animal = new[] { "Animal not found." } } });

            var entity = new UserAnimalInfoUnlocked
            {
                UnlockId = Guid.NewGuid(),
                UserId = dto.UserId,
                AnimalId = dto.AnimalId,
                InfoType = dto.InfoType,
                IsUnlocked = dto.IsUnlocked
            };

            _context.UserAnimalInfoUnlocked.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAnimalInfoUnlocked", new { id = entity.UnlockId }, entity);
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
