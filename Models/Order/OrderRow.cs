using TMA_Warehouse_solution.Models.Item;
using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Order
{
    public class OrderRow
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Item.Item? Item { get; set; }
        [Required]
        public ItemMeasurement? Measurement { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public float Price { get; set; }
        public string? Comment { get; set; }
    }

}
