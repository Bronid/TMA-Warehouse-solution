﻿using Microsoft.AspNetCore.Identity;
using TMA_Warehouse_solution.Models.Item;
using System.ComponentModel.DataAnnotations;

namespace TMA_Warehouse_solution.Models.Order
{
    public class Order
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public IdentityUser? Employee { get; set; }
        [Required]
        public OrderStatus? Status { get; set; }
        [Required]
        public List<OrderRow>? RequestRows { get; set; }
        public string? Comment { get; set; }
    }

}
