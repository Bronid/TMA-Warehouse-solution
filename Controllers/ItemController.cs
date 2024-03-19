using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            return RedirectToAction("AddItemGroup");
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

            return RedirectToAction("AddItem");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItemById(Guid id)
        {
            var item = await _context.itemModels.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.itemModels.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.itemModels.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
