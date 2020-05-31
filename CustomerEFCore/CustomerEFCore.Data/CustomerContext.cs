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
      
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>().HasKey(o => new { o.OrderId, o.ProductId });
        }

        // Uncomment below line incase if you want to create/modify dgml file
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder
        //        .UseSqlServer("Data Source=(local)\\SQLexpress;Initial Catalog=CustomerEFCoreDB;Integrated Security=True");
        //}

    }
}
