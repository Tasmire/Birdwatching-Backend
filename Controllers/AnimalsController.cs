using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final_Project_Backend.Controllers
{
    [Authorize(Roles = "Staff, Admin")]
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AnimalsController(ApplicationDbContext context) => _context = context;

        // GET: Animals
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Animals.Include(a => a.Environment);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animals = await _context.Animals
                .Include(a => a.Environment)
                .FirstOrDefaultAsync(m => m.AnimalId == id);
            if (animals == null)
            {
                return NotFound();
            }

            return View(animals);
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            // populate environment select AND spawn locations multi-select
            ViewData["EnvironmentId"] = new SelectList(_context.Environments.OrderBy(e => e.Name).ToList(), "EnvironmentId", "Name");
            ViewBag.SpawnLocationIds = new MultiSelectList(_context.SpawnLocations.OrderBy(s => s.Name).ToList(), "SpawnLocationId", "Name");
            return View();
        }

        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animals animals, List<Guid>? selectedSpawnIds)
        {
            if (ModelState.IsValid)
            {
                // attach selected spawns
                if (selectedSpawnIds != null)
                {
                    foreach (var id in selectedSpawnIds.Distinct())
                    {
                        var spawn = await _context.SpawnLocations.FindAsync(id);
                        if (spawn != null) animals.SpawnLocations.Add(spawn);
                    }
                }

                _context.Add(animals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // repopulate selects when returning the view due to error
            ViewData["EnvironmentId"] = new SelectList(_context.Environments.OrderBy(e => e.Name).ToList(), "EnvironmentId", "Name", animals.EnvironmentId);
            ViewBag.SpawnLocationIds = new MultiSelectList(_context.SpawnLocations.OrderBy(s => s.Name).ToList(), "SpawnLocationId", "Name", selectedSpawnIds);
            return View(animals);
        }

        // GET: Animals/Edit/{id}
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals
                .Include(a => a.SpawnLocations)
                .FirstOrDefaultAsync(a => a.AnimalId == id);
            if (animal == null) return NotFound();

            var selected = animal.SpawnLocations.Select(s => s.SpawnLocationId).ToList();
            ViewBag.SpawnLocationIds = new MultiSelectList(_context.SpawnLocations.OrderBy(s => s.Name).ToList(), "SpawnLocationId", "Name", selected);

            // populate environment select and preselect current environment
            ViewData["EnvironmentId"] = new SelectList(_context.Environments.OrderBy(e => e.Name).ToList(), "EnvironmentId", "Name", animal.EnvironmentId);

            return View(animal);
        }

        // POST: Animals/Edit/{id}
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Animals animals, List<Guid>? selectedSpawnIds)
        {
            if (id != animals.AnimalId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // load existing entity including navigation
                    var animalToUpdate = await _context.Animals
                        .Include(a => a.SpawnLocations)
                        .FirstOrDefaultAsync(a => a.AnimalId == id);

                    if (animalToUpdate == null) return NotFound();

                    // update scalar props (you can be selective or use TryUpdateModelAsync)
                    animalToUpdate.Name = animals.Name;
                    animalToUpdate.MaoriName = animals.MaoriName;
                    animalToUpdate.ScientificName = animals.ScientificName;
                    animalToUpdate.AverageSize = animals.AverageSize;
                    animalToUpdate.Habitat = animals.Habitat;
                    animalToUpdate.Diet = animals.Diet;
                    animalToUpdate.Origin = animals.Origin;
                    animalToUpdate.ImageUrl = animals.ImageUrl;
                    animalToUpdate.EnvironmentId = animals.EnvironmentId;

                    // update many-to-many: clear and re-add selected
                    animalToUpdate.SpawnLocations.Clear();
                    if (selectedSpawnIds != null)
                    {
                        foreach (var spawnId in selectedSpawnIds.Distinct())
                        {
                            var spawn = await _context.SpawnLocations.FindAsync(spawnId);
                            if (spawn != null) animalToUpdate.SpawnLocations.Add(spawn);
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Animals.Any(e => e.AnimalId == animals.AnimalId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // repopulate selects when returning the view due to error
            ViewData["EnvironmentId"] = new SelectList(_context.Environments.OrderBy(e => e.Name).ToList(), "EnvironmentId", "Name", animals.EnvironmentId);
            ViewBag.SpawnLocationIds = new MultiSelectList(_context.SpawnLocations.OrderBy(s => s.Name).ToList(), "SpawnLocationId", "Name", selectedSpawnIds);
            return View(animals);
        }

        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animals = await _context.Animals
                .Include(a => a.Environment)
                .FirstOrDefaultAsync(m => m.AnimalId == id);
            if (animals == null)
            {
                return NotFound();
            }

            return View(animals);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var animals = await _context.Animals.FindAsync(id);
            if (animals != null)
            {
                _context.Animals.Remove(animals);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalsExists(Guid id)
        {
            return _context.Animals.Any(e => e.AnimalId == id);
        }
    }
}
