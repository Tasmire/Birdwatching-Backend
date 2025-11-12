using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using System.Diagnostics;

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
            var applicationDbContext = _context.SpawnLocations
                .Include(s => s.Animal)
                .ThenInclude(a => a.Environment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SpawnLocations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animal)
                .ThenInclude(a => a.Environment)
                .FirstOrDefaultAsync(m => m.SpawnLocationId == id);
            if (spawnLocations == null) return NotFound();

            return View(spawnLocations);
        }

        // GET: SpawnLocations/Create
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "Name");
            return View();
        }

        // POST: SpawnLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpawnLocationId,Name,SpawnType,XCoordinate,YCoordinate,Scale,AnimalId")] SpawnLocations spawnLocations)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    spawnLocations.SpawnLocationId = Guid.NewGuid();
                    _context.Add(spawnLocations);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("SaveChanges error: " + ex.Message);
                    ModelState.AddModelError("", "Error saving to database: " + ex.Message);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                         .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? (e.Exception?.Message ?? "unknown") : e.ErrorMessage)
                                         .ToList();

            if (errors.Any())
            {
                var combined = string.Join(" | ", errors);
                Debug.WriteLine("ModelState errors: " + combined);
                ModelState.AddModelError("", combined);
            }

            // repopulate select lists
            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "Name", spawnLocations?.AnimalId);
            return View(spawnLocations);
        }

        // GET: SpawnLocations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var spawnLocations = await _context.SpawnLocations.FindAsync(id);
            if (spawnLocations == null) return NotFound();

            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "Name", spawnLocations.AnimalId);
            return View(spawnLocations);
        }

        // POST: SpawnLocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SpawnLocationId,Name,SpawnType,XCoordinate,YCoordinate,Scale,AnimalId")] SpawnLocations spawnLocations)
        {
            if (id != spawnLocations.SpawnLocationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spawnLocations);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpawnLocationsExists(spawnLocations.SpawnLocationId)) return NotFound();
                    else throw;
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                         .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? (e.Exception?.Message ?? "unknown") : e.ErrorMessage)
                                         .ToList();
            if (errors.Any()) ModelState.AddModelError("", string.Join(" | ", errors));

            ViewData["AnimalId"] = new SelectList(_context.Animals, "AnimalId", "Name", spawnLocations.AnimalId);
            return View(spawnLocations);
        }

        // GET: SpawnLocations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animal)
                .ThenInclude(a => a.Environment)
                .FirstOrDefaultAsync(m => m.SpawnLocationId == id);
            if (spawnLocations == null) return NotFound();

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
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SpawnLocationsExists(Guid id)
        {
            return _context.SpawnLocations.Any(e => e.SpawnLocationId == id);
        }
    }
}
