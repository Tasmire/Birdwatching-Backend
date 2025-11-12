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
    public class UserAnimalsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserAnimalsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserAnimalsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAnimals>>> GetUserAnimals([FromQuery] Guid? userId, [FromQuery] Guid? animalId)
        {
            IQueryable<UserAnimals> query = _context.UserAnimals;

            if (userId.HasValue)
            {
                query = query.Where(ua => ua.UserId == userId.Value);
            }

            if (animalId.HasValue)
            {
                query = query.Where(ua => ua.AnimalId == animalId.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/UserAnimalsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAnimals>> GetUserAnimals(Guid id)
        {
            var userAnimals = await _context.UserAnimals.FindAsync(id);

            if (userAnimals == null)
            {
                return NotFound();
            }

            return userAnimals;
        }

        // PUT: api/UserAnimalsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAnimals(Guid id, UserAnimalsUpdateDto dto)
        {
            if (dto == null) return BadRequest();

            var userAnimals = await _context.UserAnimals.FindAsync(id);
            if (userAnimals == null) return NotFound();

            // update only allowed fields
            userAnimals.TimesSpotted = dto.TimesSpotted;

            _context.Entry(userAnimals).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAnimalsExists(id))
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

        // POST: api/UserAnimalsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAnimals>> PostUserAnimals(UserAnimalsDto dto)
        {
            if (dto == null) return BadRequest();

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { errors = new { User = new[] { "User not found." } } });
            }

            var animal = await _context.Animals.FindAsync(dto.AnimalId);
            if (animal == null)
            {
                return BadRequest(new { errors = new { Animal = new[] { "Animal not found." } } });
            }

            var userAnimals = new UserAnimals
            {
                UserAnimalId = Guid.NewGuid(),
                UserId = dto.UserId,
                AnimalId = dto.AnimalId,
                TimesSpotted = dto.TimesSpotted
            };

            _context.UserAnimals.Add(userAnimals);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAnimals", new { id = userAnimals.UserAnimalId }, userAnimals);
        }

        // DELETE: api/UserAnimalsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAnimals(Guid id)
        {
            var userAnimals = await _context.UserAnimals.FindAsync(id);
            if (userAnimals == null)
            {
                return NotFound();
            }

            _context.UserAnimals.Remove(userAnimals);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAnimalsExists(Guid id)
        {
            return _context.UserAnimals.Any(e => e.UserAnimalId == id);
        }
    }
}
