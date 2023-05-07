using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Kerberos;
using PizzaStore.Models;

namespace PizzaStore.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // configures one-to-many relationship
            modelBuilder.Entity<Pizza>().HasOne(x => x.Category).WithMany(c => c.Pizzas).HasForeignKey(p => p.CategoryId);
      

        }
    }
}
