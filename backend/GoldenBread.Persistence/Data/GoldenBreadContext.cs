using GoldenBread.Application.Abstractions.Data;
using GoldenBread.Domain.Entities;
using System.Reflection;

namespace GoldenBread.Infrastructure.Data;

public class GoldenBreadContext : DbContext, IGoldenBreadContext
{
    public GoldenBreadContext(DbContextOptions<GoldenBreadContext> options)
        : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeTask> EmployeeTasks { get; set; }
    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<IngredientBatch> IngredientBatches { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderTariff> OrderTariffs { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBatch> ProductBatches { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
