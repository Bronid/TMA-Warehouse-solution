using TMA_Warehouse_solution.Models.Item;
using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Order
{
    public class OrderStatus
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Status { get; set; }
        
    }

}
