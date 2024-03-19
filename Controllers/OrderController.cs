using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TMA_Warehouse_solution.Controllers.OrderController;
using System.Diagnostics.Metrics;
using System.Text.Json;
using TMA_Warehouse_solution.Models.Item;
using Microsoft.AspNetCore.Identity;
using TMA_Warehouse_solution.Models.Order;
using TMA_Warehouse_solution.Models.Database;
using TMA_Warehouse_solution.Extensions;
using Microsoft.EntityFrameworkCore;

namespace TMA_Warehouse_solution.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;

        public OrderController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            List<CartController.CartItem> cartItems = HttpContext.Session.GetObject<List<CartController.CartItem>>("CurrentOrder") ?? new List<CartController.CartItem>();

            List<OrderRow> orderRows = new List<OrderRow>();

            foreach (var cartItem in cartItems)
            {
                var item = await _dbContext.itemModels.Include(i => i.Measurement).FirstOrDefaultAsync(i => i.Id == cartItem.Id);

                if (item == null)
                {
                    return BadRequest($"Item with ID {cartItem.Id} not found.");
                }

                if (item.Quantity < cartItem.Quantity)
                {
                    return BadRequest($"The quantity of item {item.Id} is insufficient.");
                }

                OrderRow orderRow = new OrderRow
                {
                    Id = Guid.NewGuid(),
                    Item = item,
                    Quantity = cartItem.Quantity,
                    Comment = cartItem.Comment,
                    Price = item.Price
                };

                orderRows.Add(orderRow);
            }


            Order order = new Order
            {
                Id = Guid.NewGuid(),
                Employee = await _userManager.GetUserAsync(User),
                RequestRows = orderRows,
                Status = OrderStatus.NEW
            };


            _dbContext.orderModels.Add(order);
            await _dbContext.SaveChangesAsync();

            HttpContext.Session.Remove("CurrentOrder");

            return RedirectToAction("List", "Cart");
        }

    }
}
    
