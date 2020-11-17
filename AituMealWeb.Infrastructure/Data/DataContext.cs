using AituMealWeb.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AituMealWeb.Infrastructure.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<MealCategory> MealCategories { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<User> UserList { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetails>()
                .HasKey(x => new { x.Id, x.MealId});
        }
    }
}
