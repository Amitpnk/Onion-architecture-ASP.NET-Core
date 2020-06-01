using CustomerEFCore.Domain;
using Microsoft.EntityFrameworkCore;

namespace CustomerEFCore.Data
{
    public class CustomerContext : DbContext
    {
        // This constructor is used of runit testing
        public CustomerContext()
        {

        }
        public CustomerContext(DbContextOptions options)
      : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        //public CustomerContext(DbContextOptions<CustomerContext> options)
        //    : base(options)
        //{
        //    ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        //}

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>().HasKey(o => new { o.OrderId, o.ProductId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                .UseSqlServer("DataSource=app.db");
            }

        }

    }
}
