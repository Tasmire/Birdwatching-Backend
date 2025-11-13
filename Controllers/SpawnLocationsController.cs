using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Final_Project_Backend.Controllers
{
    [Authorize(Roles = "Staff, Admin")]
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
                .Include(s => s.Animals)
                .ThenInclude(a => a.Environment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SpawnLocations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animals)
                .ThenInclude(a => a.Environment)
                .FirstOrDefaultAsync(m => m.SpawnLocationId == id);
            if (spawnLocations == null) return NotFound();

            return View(spawnLocations);
        }

        // GET: SpawnLocations/Create
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new MultiSelectList(_context.Animals.OrderBy(a => a.Name).ToList(), "AnimalId", "Name");
            return View();
        }

        // POST: SpawnLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpawnLocationId,Name,SpawnType,XCoordinate,YCoordinate,Scale")] SpawnLocations spawnLocations, List<Guid>? selectedAnimalIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    spawnLocations.SpawnLocationId = Guid.NewGuid();

                    // attach selected animals (many-to-many)
                    if (selectedAnimalIds != null && selectedAnimalIds.Any())
                    {
                        foreach (var aid in selectedAnimalIds.Distinct())
                        {
                            var animal = await _context.Animals.FindAsync(aid);
                            if (animal != null) spawnLocations.Animals.Add(animal);
                        }
                    }

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

            // repopulate select lists (MultiSelectList)
            ViewData["AnimalId"] = new MultiSelectList(_context.Animals.OrderBy(a => a.Name).ToList(), "AnimalId", "Name", selectedAnimalIds);
            return View(spawnLocations);
        }

        // GET: SpawnLocations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animals)
                .FirstOrDefaultAsync(s => s.SpawnLocationId == id);
            if (spawnLocations == null) return NotFound();

            var selected = spawnLocations.Animals.Select(a => a.AnimalId).ToList();
            ViewData["AnimalId"] = new MultiSelectList(_context.Animals.OrderBy(a => a.Name).ToList(), "AnimalId", "Name", selected);
            return View(spawnLocations);
        }

        // POST: SpawnLocations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("SpawnLocationId,Name,SpawnType,XCoordinate,YCoordinate,Scale")] SpawnLocations spawnLocations, List<Guid>? selectedAnimalIds)
        {
            if (id != spawnLocations.SpawnLocationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // load existing entity including navigation
                    var spawnToUpdate = await _context.SpawnLocations
                        .Include(s => s.Animals)
                        .FirstOrDefaultAsync(s => s.SpawnLocationId == id);

                    if (spawnToUpdate == null) return NotFound();

                    // update scalar props
                    spawnToUpdate.Name = spawnLocations.Name;
                    spawnToUpdate.SpawnType = spawnLocations.SpawnType;
                    spawnToUpdate.XCoordinate = spawnLocations.XCoordinate;
                    spawnToUpdate.YCoordinate = spawnLocations.YCoordinate;
                    spawnToUpdate.Scale = spawnLocations.Scale;

                    // update many-to-many: clear and re-add selected
                    spawnToUpdate.Animals.Clear();
                    if (selectedAnimalIds != null && selectedAnimalIds.Any())
                    {
                        foreach (var aid in selectedAnimalIds.Distinct())
                        {
                            var animal = await _context.Animals.FindAsync(aid);
                            if (animal != null) spawnToUpdate.Animals.Add(animal);
                        }
                    }

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

            ViewData["AnimalId"] = new MultiSelectList(_context.Animals.OrderBy(a => a.Name).ToList(), "AnimalId", "Name", selectedAnimalIds);
            return View(spawnLocations);
        }

        // GET: SpawnLocations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();

            var spawnLocations = await _context.SpawnLocations
                .Include(s => s.Animals)
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
