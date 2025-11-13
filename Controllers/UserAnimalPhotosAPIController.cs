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
    public class UserAnimalPhotosAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserAnimalPhotosAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserAnimalPhotosAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAnimalPhotos>>> GetUserAnimalPhotos([FromQuery] Guid? userId, [FromQuery] Guid? animalId)
        {
            // build queryable and apply optional filters
            var query = _context.UserAnimalPhotos.AsQueryable();

            if (userId.HasValue && userId != Guid.Empty)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }

            if (animalId.HasValue && animalId != Guid.Empty)
            {
                query = query.Where(p => p.AnimalId == animalId.Value);
            }

            return await query.ToListAsync();
        }

        // GET: api/UserAnimalPhotosAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAnimalPhotos>> GetUserAnimalPhotos(Guid id)
        {
            var userAnimalPhotos = await _context.UserAnimalPhotos.FindAsync(id);

            if (userAnimalPhotos == null)
            {
                return NotFound();
            }

            return userAnimalPhotos;
        }

        // PUT: api/UserAnimalPhotosAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAnimalPhotos(Guid id, UserAnimalPhotos userAnimalPhotos)
        {
            if (id != userAnimalPhotos.UserAnimalPhotoId)
            {
                return BadRequest();
            }

            _context.Entry(userAnimalPhotos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAnimalPhotosExists(id))
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

        // POST: api/UserAnimalPhotosAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAnimalPhotos>> PostUserAnimalPhotos(UserAnimalPhotosDto dto)
        {
            if (dto == null) return BadRequest();

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest(new { errors = new { User = new[] { "User not found." } } });

            var animal = await _context.Animals.FindAsync(dto.AnimalId);
            if (animal == null) return BadRequest(new { errors = new { Animal = new[] { "Animal not found." } } });

            var entity = new UserAnimalPhotos
            {
                UserAnimalPhotoId = Guid.NewGuid(),
                UserId = dto.UserId,
                AnimalId = dto.AnimalId,
                PhotoUrl = dto.PhotoUrl,
                DateUploaded = dto.DateUploaded
            };

            _context.UserAnimalPhotos.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAnimalPhotos", new { id = entity.UserAnimalPhotoId }, entity);
        }

        // DELETE: api/UserAnimalPhotosAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAnimalPhotos(Guid id)
        {
            var userAnimalPhotos = await _context.UserAnimalPhotos.FindAsync(id);
            if (userAnimalPhotos == null)
            {
                return NotFound();
            }

            _context.UserAnimalPhotos.Remove(userAnimalPhotos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAnimalPhotosExists(Guid id)
        {
            return _context.UserAnimalPhotos.Any(e => e.UserAnimalPhotoId == id);
        }
    }
}
