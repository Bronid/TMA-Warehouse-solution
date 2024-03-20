using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMA_Warehouse_solution.Models;
using TMA_Warehouse_solution.Models.Database;
using TMA_Warehouse_solution.Models.Item;

namespace TMA_Warehouse_solution.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var items = _context.itemModels.ToList();
            return View(items);
        }

        [Authorize(Roles = "Coordinator")]
        [HttpGet]
        public async Task<IActionResult> MyItems()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var items = _context.itemModels
                .Include(i => i.ContactPerson)
                .Include(i => i.ItemGroup)
                .Include(i => i.Measurement)
                .Where(item => item.ContactPerson.Id == currentUser.Id).ToList();
            return View(items);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult AddItemGroup()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult AddItemGroup(ItemGroup addItemGroup)
        {
            _context.itemGroupModels.Add(addItemGroup);
            _context.SaveChanges();

            return RedirectToAction("AddItemGroup");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult AddMeasurement()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public IActionResult AddMeasurement(ItemMeasurement addMeasurement)
        {
            _context.itemMeasurementModels.Add(addMeasurement);
            _context.SaveChanges();

            return RedirectToAction("AddMeasurement");
        }

        [Authorize(Roles = "Coordinator")]
        [HttpGet]
        public IActionResult AddItem()
        {
            ViewBag.ItemGroups = _context.itemGroupModels.ToList();
            ViewBag.Measurements = _context.itemMeasurementModels.ToList();
            return View();
        }

        [Authorize(Roles = "Coordinator")]
        [HttpPost]
        public async Task<IActionResult> AddItem(Item addItem, Guid itemGroupId, Guid measurementId)
        {
            addItem.ContactPerson = await _userManager.GetUserAsync(User);

            var itemGroup = _context.itemGroupModels.Find(itemGroupId);
            var measurement = _context.itemMeasurementModels.Find(measurementId);

            addItem.ItemGroup = itemGroup;
            addItem.Measurement = measurement;

            _context.itemModels.Add(addItem);
            _context.SaveChanges();

            return RedirectToAction("MyItems");
        }

        [Authorize(Roles = "Coordinator")]
        [HttpGet]
        public async Task<IActionResult> EditItem(Guid id)
        {
            var item = _context.itemModels.Include(i => i.ContactPerson).First(predicate => predicate.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (item.ContactPerson.Id != currentUser.Id)
            {
                return Forbid();
            }

            ViewBag.ItemGroups = _context.itemGroupModels.ToList();
            ViewBag.Measurements = _context.itemMeasurementModels.ToList();

            return View(item);
        }

        [Authorize(Roles = "Coordinator")]
        [HttpPut]
        public async Task<IActionResult> EditItem(Guid id, string name, int quantity, float price, string status, string storageLocation)
        {
            var item = await _context.itemModels.Include(i => i.ContactPerson).FirstOrDefaultAsync(predicate => predicate.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (item.ContactPerson.Id != currentUser.Id)
            {
                return Forbid();
            }

            item.Name = name;
            item.Quantity = quantity;
            item.Price = price;
            item.Status = status;
            item.StorageLocation = storageLocation;

            _context.SaveChanges();

            return Ok(new { success = true });
        }


        [Authorize(Roles = "Coordinator")]
        [HttpDelete]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = _context.itemModels.Include(i => i.ContactPerson).First(predicate => predicate.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (item.ContactPerson.Id != currentUser.Id)
            {
                return Forbid();
            }

            _context.itemModels.Remove(item);
            _context.SaveChanges();

            return Ok(new { success = true });
        }
    }
}
