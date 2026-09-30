namespace ProductInventoryManager.Core.Models;

/// <summary>
/// Represents a product entity stored within the inventory dictionary.
/// Stores core attributes required by the assignment (ProductID, Name, Price)
/// alongside supplementary metadata (Category, StockQuantity, Description, DateAdded).
/// </summary>
public class Product
{
    public int ProductID { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = "General";
    public int StockQuantity { get; set; } = 1;
    public string Description { get; set; } = string.Empty;
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Formatted price in South African Rand (ZAR) as required by the specification.
    /// Uses InvariantCulture for consistent deterministic formatting (e.g. R 5,000.00).
    /// </summary>
    public string FormattedPrice => $"R {Price.ToString("N2", System.Globalization.CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Formatted badge display ID.
    /// </summary>
    public string FormattedId => $"#{ProductID}";

    /// <summary>
    /// Formatted display for stock quantity.
    /// </summary>
    public string FormattedStock => $"{StockQuantity} unit(s)";

    public Product()
    {
    }

    public Product(int id, string name, decimal price, string category = "General", int stockQuantity = 1, string description = "")
    {
        ProductID = id;
        Name = name?.Trim() ?? string.Empty;
        Price = price;
        Category = string.IsNullOrWhiteSpace(category) ? "General" : category.Trim();
        StockQuantity = Math.Max(0, stockQuantity);
        Description = description?.Trim() ?? string.Empty;
        DateAdded = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates business rules for the product entity.
    /// </summary>
    public (bool IsValid, string? ErrorMessage) Validate()
    {
        if (ProductID <= 0)
        {
            return (false, "Product ID must be a positive integer greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            return (false, "Product name cannot be empty or whitespace.");
        }

        if (Price < 0)
        {
            return (false, "Product price cannot be negative.");
        }

        if (StockQuantity < 0)
        {
            return (false, "Stock quantity cannot be negative.");
        }

        return (true, null);
    }

    /// <summary>
    /// Creates a deep copy of this product instance.
    /// </summary>
    public Product Clone()
    {
        return new Product
        {
            ProductID = ProductID,
            Name = Name,
            Price = Price,
            Category = Category,
            StockQuantity = StockQuantity,
            Description = Description,
            DateAdded = DateAdded
        };
    }

    public override string ToString()
    {
        return $"[ID: {ProductID}] {Name,-25} | Price: {FormattedPrice,-12} | Category: {Category,-15} | Stock: {StockQuantity}";
    }
}
