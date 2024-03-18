using TMA_Warehouse_solution.Models.Item;
using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Order
{
    public class OrderStatus
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Status { get; set; }
        
    }

}
