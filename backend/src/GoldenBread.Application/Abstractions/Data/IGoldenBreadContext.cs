using GoldenBread.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GoldenBread.Application.Abstractions.Data;

public interface IGoldenBreadContext
{
    DbSet<Account> Accounts { get; set; }
    DbSet<CartItem> CartItems { get; set; }
    DbSet<Company> Companies { get; set; }
    DbSet<Employee> Employees { get; set; }
    DbSet<EmployeeTask> EmployeeTasks { get; set; }
    DbSet<Favorite> Favorites { get; set; }
    DbSet<Ingredient> Ingredients { get; set; }
    DbSet<SupplierIngredient> SupplierIngredients { get; set; }
    DbSet<OrderItemIngredientReservation> OrderItemIngredientReservations { get; set; }
    DbSet<IngredientBatch> IngredientBatches { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<OrderItem> OrderItems { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<ProductBatch> ProductBatches { get; set; }
    DbSet<ProductCategory> ProductCategories { get; set; }
    DbSet<ProductImage> ProductImages { get; set; }
    DbSet<Recipe> Recipes { get; set; }
    DbSet<Supplier> Suppliers { get; set; }
    DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken ct);
    DatabaseFacade Database { get; }
}
