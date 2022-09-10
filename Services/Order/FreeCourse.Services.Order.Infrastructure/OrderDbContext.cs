using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Infrastructure
{
    /// <summary>
    /// Database Connection
    /// </summary>
    public class OrderDbContext:DbContext
    {
        public const string DEFAULT_SHEMA = "ordering";

        #region Constructor
        public OrderDbContext(DbContextOptions<OrderDbContext> options):base(options)
        {

        }
        #endregion

        #region DbSets(Database Tables)
        public DbSet<Domain.OrderAggregate.Order> Orders { get; set; }
        public DbSet<Domain.OrderAggregate.OrderItem> OrderItems { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SHEMA);
            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().ToTable("OrderItems", DEFAULT_SHEMA);

            modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2");
            modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(o => o.Address).WithOwner(); //by Ownered all Address properties created automatically to Order Tabe and also create OrderId into OrderItem table for one to many realtion
            base.OnModelCreating(modelBuilder);
        }
    }
}
