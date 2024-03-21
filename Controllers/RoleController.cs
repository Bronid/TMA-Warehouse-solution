using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace TMA_Warehouse_solution.Controllers
{

    [Authorize(Roles = "Administrator")]
    public class RoleController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RoleController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Edit()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> SetEmployeeRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveFromRoleAsync(user, "Coordinator");

            await _userManager.AddToRoleAsync(user, "Employee");

            return RedirectToAction("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> SetCoordinatorRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveFromRoleAsync(user, "Employee");

            await _userManager.AddToRoleAsync(user, "Coordinator");

            return RedirectToAction("Edit");
        }
    }
}
