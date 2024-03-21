using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using TMA_Warehouse_solution.Models;
using TMA_Warehouse_solution.Models.Database;
using TMA_Warehouse_solution.Models.Item;

namespace TMA_Warehouse_solution.Controllers
{
    public class ItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ItemController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var items = _context.itemModels.ToList();
            return View(items);
        }

        [Authorize(Roles = "Coordinator, Administrator")]
        [HttpGet]
        public async Task<IActionResult> MyItems()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (User.IsInRole("Administrator"))
            {
                var items = _context.itemModels
                .Include(i => i.ContactPerson)
                .Include(i => i.ItemGroup)
                .Include(i => i.Measurement)
                .ToList();
                return View(items);
            }
            else
            {
                var items = _context.itemModels
                .Include(i => i.ContactPerson)
                .Include(i => i.ItemGroup)
                .Include(i => i.Measurement)
                .Where(item => item.ContactPerson.Id == currentUser.Id).ToList();
                return View(items);
            }

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

        [Authorize(Roles = "Coordinator, Administrator")]
        [HttpGet]
        public IActionResult AddItem()
        {
            ViewBag.ItemGroups = _context.itemGroupModels.ToList();
            ViewBag.Measurements = _context.itemMeasurementModels.ToList();
            return View();
        }

        [Authorize(Roles = "Coordinator, Administrator")]
        [HttpPost]
        public async Task<IActionResult> AddItem(Item addItem, Guid itemGroupId, Guid measurementId)
        {
            addItem.ContactPerson = await _userManager.GetUserAsync(User);

            var itemGroup = _context.itemGroupModels.Find(itemGroupId);
            var measurement = _context.itemMeasurementModels.Find(measurementId);

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            if (addItem.Photo != null && addItem.Photo.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(addItem.Photo.FileName);
                var imagePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await addItem.Photo.CopyToAsync(stream);
                }

                addItem.ImagePath = "/images/" + fileName;
            }

            addItem.ItemGroup = itemGroup;
            addItem.Measurement = measurement;

            _context.itemModels.Add(addItem);
            _context.SaveChanges();

            return RedirectToAction("MyItems");
        }



        [Authorize(Roles = "Coordinator, Administrator")]
        [HttpGet]
        public async Task<IActionResult> EditItem(Guid id)
        {
            var item = _context.itemModels.Include(i => i.ContactPerson).First(predicate => predicate.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (item.ContactPerson.Id != currentUser.Id && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            ViewBag.ItemGroups = _context.itemGroupModels.ToList();
            ViewBag.Measurements = _context.itemMeasurementModels.ToList();

            return View(item);
        }

        [Authorize(Roles = "Coordinator, Administrator")]
        [HttpPut]
        public async Task<IActionResult> EditItem(Guid id, string name, int quantity, float price, string status, string storageLocation, IFormFile photo)
        {
            var item = await _context.itemModels.Include(i => i.ContactPerson).FirstOrDefaultAsync(predicate => predicate.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (item.ContactPerson.Id != currentUser.Id && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            if (photo != null && photo.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                var imagePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                item.ImagePath = "/images/" + fileName;
            }

            item.Name = name;
            item.Quantity = quantity;
            item.Price = price;
            item.Status = status;
            item.StorageLocation = storageLocation;

            _context.SaveChanges();

            return Ok(new { success = true });
        }



        [Authorize(Roles = "Coordinator, Administrator")]
        [HttpDelete]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = _context.itemModels.Include(i => i.ContactPerson).First(predicate => predicate.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (item.ContactPerson.Id != currentUser.Id && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            _context.itemModels.Remove(item);
            _context.SaveChanges();

            return Ok(new { success = true });
        }
    }
}
