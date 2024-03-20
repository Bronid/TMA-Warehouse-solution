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
using Microsoft.AspNetCore.Authorization;
using System.Data;

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Coordinator"))
            {
                 var orders = _dbContext.orderModels
                .Include(o => o.RequestRows)
                    .ThenInclude(rr => rr.Item)
                        .ThenInclude(i => i.ItemGroup)
                .Include(o => o.RequestRows)
                    .ThenInclude(rr => rr.Item)
                        .ThenInclude(i => i.Measurement)
                .Include(o => o.Employee)
                .Where(o => o.RequestRows.Any(r => r.Item.ContactPerson.Id == currentUser.Id))
                .ToList();

                return View(orders);
            }

            else if(User.IsInRole("Employee"))
            {
                var orders = _dbContext.orderModels
                .Include(o => o.RequestRows)
                    .ThenInclude(rr => rr.Item)
                        .ThenInclude(i => i.ItemGroup)
                .Include(o => o.RequestRows)
                    .ThenInclude(rr => rr.Item)
                        .ThenInclude(i => i.Measurement)
                .Include(o => o.Employee)
                .Where(o => o.Employee == currentUser)
                .ToList();

                return View(orders);
            }

            return View();
        }

        [Authorize(Roles = "Employee")]
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

        [Authorize(Roles = "Coordinator")]
        [HttpPost]
        public async Task<IActionResult> ApproveOrder(Guid orderId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var order = await _dbContext.orderModels
                .Include(o => o.RequestRows)
                    .ThenInclude(rr => rr.Item)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            if (!order.RequestRows.Any(rr => rr.Item.ContactPerson.Id == currentUser.Id))
            {
                return Forbid();
            }

            foreach (var orderRow in order.RequestRows)
            {
                var item = orderRow.Item;

                if (item.Quantity < orderRow.Quantity)
                {
                    return BadRequest($"The quantity of item {item.Id} is insufficient.");
                }

                item.Quantity -= orderRow.Quantity;
            }

            order.Status = OrderStatus.APPROVE;

            _dbContext.orderModels.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Coordinator")]
        [HttpPost]
        public async Task<IActionResult> RejectOrder(Guid orderId, string comment)
        {
            var order = await _dbContext.orderModels
                .Include(o => o.RequestRows)
                .ThenInclude(r => r.Item)
                .ThenInclude(i => i.ContactPerson)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            var currentUser = await _userManager.GetUserAsync(User);

            if (!order.RequestRows.Any(rr => rr.Item.ContactPerson == currentUser))
            {
                return Forbid();
            }

            if (order == null)
            {
                return NotFound($"Order with ID {orderId} not found.");
            }

            order.Status = OrderStatus.REJECT;
            order.Comment = comment;

            _dbContext.orderModels.Update(order);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }


    }
}
    
