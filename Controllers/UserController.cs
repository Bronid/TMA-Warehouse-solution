using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TMA_Warehouse_solution.Models.Database;

namespace TMA_Warehouse_solution.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IdentityDbContext<IdentityUser> _userContext;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            var currentUser = _userContext.Users.Find(User.Identity.Name);
            var requestedUser = _userContext.Users.Find(id);

            if (currentUser == null || requestedUser == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Administrator") || currentUser.Id == requestedUser.Id)
            {
                return View(requestedUser);
            }
            else
            {
                return Forbid();
            }
        }

        // POST: UserController/Create Регистрация юзера
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
