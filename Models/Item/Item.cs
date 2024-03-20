using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Item
{
    public class Item
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public ItemGroup? ItemGroup { get; set; }
        [Required]
        public ItemMeasurement? Measurement { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public string? Status { get; set; } //TODO: Create Status model
        public string? StorageLocation { get; set; } //TODO: Create Location model
        public IdentityUser? ContactPerson { get; set; }
        public string? Photo { get; set; } //URL to photo

        public Item()
        {
            Id = Guid.NewGuid();
        }
    }

}
