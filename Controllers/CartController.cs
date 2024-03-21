using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMA_Warehouse_solution.Extensions;
using TMA_Warehouse_solution.Models.Database;
using TMA_Warehouse_solution.Models.Item;

namespace TMA_Warehouse_solution.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext dbcontext)
        {
            _context = dbcontext;
        }

        public class CartItem
        {
            public Guid Id { get; set; }
            public int Quantity { get; set; }
            public string? Comment { get; set; }
        }

        public class OrderViewModel
        {
            public List<CartItem> CurrentOrder { get; set; }
            public List<Item> ItemsInCurrentOrder { get; set; }
        }

        public IActionResult AddToCart(Guid itemId, int quantity, string comment)
        {
            CartItem newCartItem = new CartItem
            {
                Id = itemId,
                Quantity = quantity,
                Comment = comment
            };

            List<CartItem> currentOrder = HttpContext.Session.GetObject<List<CartItem>>("CurrentOrder") ?? new List<CartItem>();
            currentOrder.Add(newCartItem);
            HttpContext.Session.SetObject("CurrentOrder", currentOrder);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult List()
        {
            List<CartItem> currentOrder = HttpContext.Session.GetObject<List<CartItem>>("CurrentOrder") ?? new List<CartItem>();
            var itemIdsInCurrentOrder = currentOrder.Select(o => o.Id).ToList();
            var itemsInCurrentOrder = _context.itemModels.Where(i => itemIdsInCurrentOrder.Contains(i.Id)).ToList();

            var viewModel = new OrderViewModel
            {
                CurrentOrder = currentOrder,
                ItemsInCurrentOrder = itemsInCurrentOrder
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateCartItem(Guid itemId, int quantity, string comment)
        {
            List<CartItem> currentOrder = HttpContext.Session.GetObject<List<CartItem>>("CurrentOrder") ?? new List<CartItem>();
            CartItem cartItemToUpdate = currentOrder.FirstOrDefault(item => item.Id == itemId);
            if (cartItemToUpdate != null)
            {
                cartItemToUpdate.Quantity = quantity;
                cartItemToUpdate.Comment = comment;
                HttpContext.Session.SetObject("CurrentOrder", currentOrder);
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult RemoveCartItem(Guid itemId)
        {
            List<CartItem> currentOrder = HttpContext.Session.GetObject<List<CartItem>>("CurrentOrder") ?? new List<CartItem>();
            CartItem cartItemToRemove = currentOrder.FirstOrDefault(item => item.Id == itemId);
            if (cartItemToRemove != null)
            {
                currentOrder.Remove(cartItemToRemove);
                HttpContext.Session.SetObject("CurrentOrder", currentOrder);
            }
            return RedirectToAction("List");
        }
    }


}
