using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using Environments = Final_Project_Backend.Models.Environments;

namespace Final_Project_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvironmentsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnvironmentsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EnvironmentsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Environments>>> GetEnvironments()
        {
            return await _context.Environments.ToListAsync();
        }

        // GET: api/EnvironmentsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Environments>> GetEnvironments(Guid id)
        {
            var environments = await _context.Environments.FindAsync(id);

            if (environments == null)
            {
                return NotFound();
            }

            return environments;
        }

        // PUT: api/EnvironmentsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnvironments(Guid id, Environments environments)
        {
            if (id != environments.EnvironmentId)
            {
                return BadRequest();
            }

            _context.Entry(environments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EnvironmentsExists(id))
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

        // POST: api/EnvironmentsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Environments>> PostEnvironments(Environments environments)
        {
            _context.Environments.Add(environments);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEnvironments", new { id = environments.EnvironmentId }, environments);
        }

        // DELETE: api/EnvironmentsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvironments(Guid id)
        {
            var environments = await _context.Environments.FindAsync(id);
            if (environments == null)
            {
                return NotFound();
            }

            _context.Environments.Remove(environments);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EnvironmentsExists(Guid id)
        {
            return _context.Environments.Any(e => e.EnvironmentId == id);
        }
    }
}
