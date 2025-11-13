using Final_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Final_Project_Backend.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly UserManager<Users> _userManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(RoleManager<IdentityRole<Guid>> roleManager, UserManager<Users> userManager, ILogger<RolesController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var model = new List<RoleInfoViewModel>(roles.Count);

            foreach (var r in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(r.Name ?? string.Empty);
                model.Add(new RoleInfoViewModel
                {
                    RoleId = r.Id.ToString(),
                    RoleName = r.Name ?? string.Empty,
                    MemberCount = usersInRole?.Count ?? 0
                });
            }

            return View(model);
        }

        // GET: /Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolesCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var roleName = model.RoleName.Trim();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };
                var res = await _roleManager.CreateAsync(role);
                if (!res.Succeeded)
                {
                    ModelState.AddModelError("", string.Join("; ", res.Errors.Select(e => e.Description)));
                    return View(model);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Roles/Assign
        public async Task<IActionResult> Assign()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var vm = new RolesAssignViewModel
            {
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(u.UserName) ? (u.Email ?? u.Id.ToString()) : u.UserName
                }),
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name ?? string.Empty,
                    Text = r.Name ?? string.Empty
                }),
                SelectedRoles = new List<string>() // empty by default; will be populated when a user is selected on the client or on POST
            };

            return View(vm);
        }

        // POST: /Roles/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(RolesAssignViewModel model)
        {
            // Rebuild selects for redisplay if needed
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            model.Users = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = string.IsNullOrWhiteSpace(u.UserName) ? (u.Email ?? u.Id.ToString()) : u.UserName
            });
            model.Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Name ?? string.Empty,
                Text = r.Name ?? string.Empty
            });

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selected = model.SelectedRoles ?? new List<string>();

            // compute roles to add and remove
            var toAdd = selected.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToArray();
            var toRemove = currentRoles.Except(selected, StringComparer.OrdinalIgnoreCase).ToArray();

            if (toRemove.Any())
            {
                var remResult = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!remResult.Succeeded)
                {
                    var err = string.Join("; ", remResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed removing roles from {UserId}: {Errors}", model.UserId, err);
                    ModelState.AddModelError("", err);
                    return View(model);
                }
            }

            if (toAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, toAdd);
                if (!addResult.Succeeded)
                {
                    var err = string.Join("; ", addResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed adding roles to {UserId}: {Errors}", model.UserId, err);
                    ModelState.AddModelError("", err);
                    return View(model);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
