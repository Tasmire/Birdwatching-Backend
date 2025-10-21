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
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimalsController(ApplicationDbContext context)
        {
            _context = context;
        }

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
            ViewData["EnvironmentId"] = new SelectList(_context.Environments, "EnvironmentId", "Name");
            return View();
        }

        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AnimalId,Name,MaoriName,ScientificName,AverageSize,Habitat,Diet,Origin,ImageUrl,EnvironmentId")] Animals animals)
        {
            if (ModelState.IsValid)
            {
                animals.AnimalId = Guid.NewGuid();
                _context.Add(animals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EnvironmentId"] = new SelectList(_context.Environments, "EnvironmentId", "EnvironmentId", animals.EnvironmentId);
            return View(animals);
        }

        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animals = await _context.Animals.FindAsync(id);
            if (animals == null)
            {
                return NotFound();
            }
            ViewData["EnvironmentId"] = new SelectList(_context.Environments, "EnvironmentId", "Name", animals.EnvironmentId);
            return View(animals);
        }

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("AnimalId,Name,MaoriName,ScientificName,AverageSize,Habitat,Diet,Origin,ImageUrl,EnvironmentId")] Animals animals)
        {
            if (id != animals.AnimalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animals);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalsExists(animals.AnimalId))
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
            ViewData["EnvironmentId"] = new SelectList(_context.Environments, "EnvironmentId", "EnvironmentId", animals.EnvironmentId);
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
