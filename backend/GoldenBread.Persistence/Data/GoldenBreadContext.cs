using GoldenBread.Application.Common.Abstractions.Data;
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
    public virtual DbSet<CartItem> CartItems { get; set; }
    public DbSet<Company> Companies { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<EmployeeTask> EmployeeTasks { get; set; }
    public virtual DbSet<Favourite> Favourites { get; set; }
    public virtual DbSet<Ingredient> Ingredients { get; set; }
    public virtual DbSet<IngredientBatch> IngredientBatches { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<OrderTariff> OrderTariffs { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductBatch> ProductBatches { get; set; }
    public virtual DbSet<ProductCategory> ProductCategories { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<Recipe> Recipes { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
