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
using Environments = Final_Project_Backend.Models.Environments;

namespace Final_Project_Backend.Controllers
{
    [Authorize(Roles = "Staff, Admin")]
    public class EnvironmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnvironmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Environments
        public async Task<IActionResult> Index()
        {
            return View(await _context.Environments.ToListAsync());
        }

        // GET: Environments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var environments = await _context.Environments
                .FirstOrDefaultAsync(m => m.EnvironmentId == id);
            if (environments == null)
            {
                return NotFound();
            }

            return View(environments);
        }

        // GET: Environments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Environments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnvironmentId,Name,NavigationIcon,ImageUrl")] Environments environments)
        {
            if (ModelState.IsValid)
            {
                environments.EnvironmentId = Guid.NewGuid();
                _context.Add(environments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(environments);
        }

        // GET: Environments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var environments = await _context.Environments.FindAsync(id);
            if (environments == null)
            {
                return NotFound();
            }
            return View(environments);
        }

        // POST: Environments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("EnvironmentId,Name,NavigationIcon,ImageUrl")] Environments environments)
        {
            if (id != environments.EnvironmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(environments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnvironmentsExists(environments.EnvironmentId))
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
            return View(environments);
        }

        // GET: Environments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var environments = await _context.Environments
                .FirstOrDefaultAsync(m => m.EnvironmentId == id);
            if (environments == null)
            {
                return NotFound();
            }

            return View(environments);
        }

        // POST: Environments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var environments = await _context.Environments.FindAsync(id);
            if (environments != null)
            {
                _context.Environments.Remove(environments);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnvironmentsExists(Guid id)
        {
            return _context.Environments.Any(e => e.EnvironmentId == id);
        }
    }
}
