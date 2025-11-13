using Final_Project_Backend.Data;
using Final_Project_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Final_Project_Backend.Controllers
{
    [Authorize(Roles = "Staff, Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var usersCount = await _context.Users.AsNoTracking().CountAsync();
            var animalsCount = await _context.Animals.AsNoTracking().CountAsync();
            var achievementsCount = await _context.Achievements.AsNoTracking().CountAsync();
            var totalUnlocked = await _context.UserAchievements.AsNoTracking().LongCountAsync();

            double avgProgressPercent = 0;
            if (usersCount > 0 && achievementsCount > 0)
            {
                avgProgressPercent = (double)totalUnlocked / (usersCount * (double)achievementsCount) * 100.0;
            }

            var animalsPerEnvironment = await _context.Environments
                .AsNoTracking()
                .Select(e => new { e.Name, Count = _context.Animals.Count(a => a.EnvironmentId == e.EnvironmentId) })
                .ToListAsync();

            var model = new DashboardStatsViewModel
            {
                RegisteredPlayers = usersCount,
                AnimalCount = animalsCount,
                TotalAchievements = achievementsCount,
                TotalUnlockedAchievements = totalUnlocked,
                AverageAchievementProgressPercent = Math.Round(avgProgressPercent, 2),
                AnimalsPerEnvironment = animalsPerEnvironment.Select(x => (x.Name, x.Count))
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
