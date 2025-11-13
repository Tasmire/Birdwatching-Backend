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
    public class SpawnLocationsAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpawnLocationsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SpawnLocationsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpawnLocations>>> GetSpawnLocations()
        {
            var list = await _context.SpawnLocations
        .Include(s => s.Animals) // ensure EF loads the relation
        .Select(s => new {
            s.SpawnLocationId,
            s.Name,
            s.SpawnType,
            s.XCoordinate,
            s.YCoordinate,
            s.Scale,
            Animals = s.Animals.Select(a => a.AnimalId).ToList() // send IDs only
        })
        .ToListAsync();

            return Ok(list);
        }

        // GET: api/SpawnLocationsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpawnLocations>> GetSpawnLocations(Guid id)
        {
            var spawnLocations = await _context.SpawnLocations.FindAsync(id);

            if (spawnLocations == null)
            {
                return NotFound();
            }

            return spawnLocations;
        }

        // PUT: api/SpawnLocationsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpawnLocations(Guid id, SpawnLocations spawnLocations)
        {
            if (id != spawnLocations.SpawnLocationId)
            {
                return BadRequest();
            }

            _context.Entry(spawnLocations).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpawnLocationsExists(id))
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

        // POST: api/SpawnLocationsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SpawnLocations>> PostSpawnLocations(SpawnLocations spawnLocations)
        {
            _context.SpawnLocations.Add(spawnLocations);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpawnLocations", new { id = spawnLocations.SpawnLocationId }, spawnLocations);
        }

        // DELETE: api/SpawnLocationsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpawnLocations(Guid id)
        {
            var spawnLocations = await _context.SpawnLocations.FindAsync(id);
            if (spawnLocations == null)
            {
                return NotFound();
            }

            _context.SpawnLocations.Remove(spawnLocations);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpawnLocationsExists(Guid id)
        {
            return _context.SpawnLocations.Any(e => e.SpawnLocationId == id);
        }
    }
}
