using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Item
{
    public class ItemGroup
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }

        public ItemGroup()
        {
            Id = Guid.NewGuid();
        }
    }

}
