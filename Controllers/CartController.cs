using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMA_Warehouse_solution.Extensions;

namespace TMA_Warehouse_solution.Controllers
{
    public class CartController : Controller
    {
        public class CartItem
        {
            public Guid Id { get; set; }
            public int Quantity { get; set; }
            public string Comment { get; set; }
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
            return View(currentOrder);
        }

    }
}
