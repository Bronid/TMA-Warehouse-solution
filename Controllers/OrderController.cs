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

namespace TMA_Warehouse_solution.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

    }

}
