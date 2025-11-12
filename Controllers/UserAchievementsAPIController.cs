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
    public class UserAchievementsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserAchievementsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserAchievementsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAchievements>>> GetUserAchievements()
        {
            return await _context.UserAchievements.ToListAsync();
        }

        // GET: api/UserAchievementsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserAchievements>> GetUserAchievements(Guid id)
        {
            var userAchievements = await _context.UserAchievements.FindAsync(id);

            if (userAchievements == null)
            {
                return NotFound();
            }

            return userAchievements;
        }

        // PUT: api/UserAchievementsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAchievements(Guid id, UserAchievementsUpdateDto dto)
        {
            if (dto == null) return BadRequest();

            var entity = await _context.UserAchievements.FindAsync(id);
            if (entity == null) return NotFound();

            entity.DateAchieved = dto.DateAchieved ?? entity.DateAchieved;

            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAchievementsExists(id))
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

        // POST: api/UserAchievementsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserAchievements>> PostUserAchievements(UserAchievementsDto dto)
        {
            if (dto == null) return BadRequest();

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return BadRequest(new { errors = new { User = new[] { "User not found." } } });

            var achievement = await _context.Achievements.FindAsync(dto.AchievementId);
            if (achievement == null) return BadRequest(new { errors = new { Achievement = new[] { "Achievement not found." } } });

            var entity = new UserAchievements
            {
                UserAchievementId = Guid.NewGuid(),
                UserId = dto.UserId,
                AchievementId = dto.AchievementId,
                DateAchieved = dto.DateAchieved
            };

            _context.UserAchievements.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserAchievements", new { id = entity.UserAchievementId }, entity);
        }

        // DELETE: api/UserAchievementsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAchievements(Guid id)
        {
            var userAchievements = await _context.UserAchievements.FindAsync(id);
            if (userAchievements == null)
            {
                return NotFound();
            }

            _context.UserAchievements.Remove(userAchievements);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserAchievementsExists(Guid id)
        {
            return _context.UserAchievements.Any(e => e.UserAchievementId == id);
        }
    }
}
