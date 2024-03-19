using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TMA_Warehouse_solution.Models.Item;
using TMA_Warehouse_solution.Models.Order;

namespace TMA_Warehouse_solution.Models.Database
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ItemGroup> itemGroupModels { get; set; }
        public DbSet<ItemMeasurement> itemMeasurementModels { get; set; }
        public DbSet<Item.Item> itemModels { get; set; }
        public DbSet<Order.Order> orderModels { get; set; }
        public DbSet<OrderRow> orderRowModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

           modelBuilder.Entity<Order.Order>()
                .HasMany(o => o.RequestRows)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderRow>()
                .HasOne(o => o.Item)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);


            base.OnModelCreating(modelBuilder);
        }


    }
}
