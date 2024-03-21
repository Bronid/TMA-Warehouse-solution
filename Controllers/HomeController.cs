using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TMA_Warehouse_solution.Models;
using TMA_Warehouse_solution.Models.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TMA_Warehouse_solution.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(string filter, string sortBy)
        {
            var items = _context.itemModels
                .Include(i => i.ItemGroup)
                .Include(i => i.Measurement)
                .Include(i => i.ContactPerson)
                .ToList();

            if (!string.IsNullOrEmpty(filter))
            {
                items = items.Where(i => i.Name.Contains(filter)).ToList();
            }

            if (sortBy == "price")
            {
                items = items.OrderBy(i => i.Price).ToList();
            }

            return View(items);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}