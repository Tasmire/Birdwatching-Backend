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
    public class AchievementsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AchievementsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AchievementsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Achievements>>> GetAchievements()
        {
            return await _context.Achievements.ToListAsync();
        }

        // GET: api/AchievementsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Achievements>> GetAchievements(Guid id)
        {
            var achievements = await _context.Achievements.FindAsync(id);

            if (achievements == null)
            {
                return NotFound();
            }

            return achievements;
        }

        // PUT: api/AchievementsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAchievements(Guid id, Achievements achievements)
        {
            if (id != achievements.AchievementId)
            {
                return BadRequest();
            }

            _context.Entry(achievements).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AchievementsExists(id))
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

        // POST: api/AchievementsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Achievements>> PostAchievements(Achievements achievements)
        {
            _context.Achievements.Add(achievements);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAchievements", new { id = achievements.AchievementId }, achievements);
        }

        // DELETE: api/AchievementsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAchievements(Guid id)
        {
            var achievements = await _context.Achievements.FindAsync(id);
            if (achievements == null)
            {
                return NotFound();
            }

            _context.Achievements.Remove(achievements);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AchievementsExists(Guid id)
        {
            return _context.Achievements.Any(e => e.AchievementId == id);
        }
    }
}
