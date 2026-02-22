using System;
using System.Collections.Generic;

namespace GoldenBread.Domain.Entities;

public class Product
{
    public int ProductId { get; private set; }

    public int CategoryId { get; private set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public int MarkupPercent { get; set; }
    public decimal Weight { get; set; }
    public int ProductionTime { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ProductCategory Category { get; set; } = null!;

    public ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();
    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();

    public Product() { }
}
