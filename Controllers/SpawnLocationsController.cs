using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_Project_Backend.Data;
using Final_Project_Backend.Models;

namespace Final_Project_Backend.Controllers
{
    public class SpawnLocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpawnLocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SpawnLocations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SpawnLocations.Include(s => s.Animal);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SpawnLocations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animal)
                .FirstOrDefaultAsync(m => m.SpawnLocationId == id);
            if (spawnLocations == null)
            {
                return NotFound();
            }

            return View(spawnLocations);
        }

        // GET: SpawnLocations/Create
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId");
            return View();
        }

        // POST: SpawnLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpawnLocationId,Name,SpawnType,XCoordinate,YCoordinate,ZCoordinate,AnimalId,EnvironmentId")] SpawnLocations spawnLocations)
        {
            if (ModelState.IsValid)
            {
                spawnLocations.SpawnLocationId = Guid.NewGuid();
                _context.Add(spawnLocations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId", spawnLocations.AnimalId);
            return View(spawnLocations);
        }

        // GET: SpawnLocations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spawnLocations = await _context.SpawnLocations.FindAsync(id);
            if (spawnLocations == null)
            {
                return NotFound();
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId", spawnLocations.AnimalId);
            return View(spawnLocations);
        }

        // POST: SpawnLocations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SpawnLocationId,Name,SpawnType,XCoordinate,YCoordinate,ZCoordinate,AnimalId,EnvironmentId")] SpawnLocations spawnLocations)
        {
            if (id != spawnLocations.SpawnLocationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spawnLocations);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpawnLocationsExists(spawnLocations.SpawnLocationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "AnimalId", spawnLocations.AnimalId);
            return View(spawnLocations);
        }

        // GET: SpawnLocations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animal)
                .FirstOrDefaultAsync(m => m.SpawnLocationId == id);
            if (spawnLocations == null)
            {
                return NotFound();
            }

            return View(spawnLocations);
        }

        // POST: SpawnLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var spawnLocations = await _context.SpawnLocations.FindAsync(id);
            if (spawnLocations != null)
            {
                _context.SpawnLocations.Remove(spawnLocations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpawnLocationsExists(Guid id)
        {
            return _context.SpawnLocations.Any(e => e.SpawnLocationId == id);
        }
    }
}
