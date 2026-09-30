using ProductInventoryManager.Core.Models;

namespace ProductInventoryManager.Core.Services;

/// <summary>
/// Defines the contract for product inventory management utilizing a C# Dictionary data structure.
/// Implements all required operations: Add, Update, Search, Delete, and Display.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Gets the count of items currently stored in the product dictionary.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Checks whether a product exists in the dictionary with the specified ProductID key.
    /// </summary>
    bool ContainsProduct(int id);

    /// <summary>
    /// Adds a new product to the dictionary. Enforces unique ProductID key constraint and data validation.
    /// </summary>
    bool AddProduct(Product product, out string errorMessage);

    /// <summary>
    /// Updates details of an existing product located by ProductID key.
    /// </summary>
    bool UpdateProduct(
        int id,
        string newName,
        decimal newPrice,
        string? newCategory,
        int? newStockQuantity,
        string? newDescription,
        out string errorMessage);

    /// <summary>
    /// Deletes a product from the dictionary by its ProductID key.
    /// </summary>
    bool DeleteProduct(int id, out string errorMessage);

    /// <summary>
    /// Searches and retrieves a product by its ProductID key via O(1) Dictionary lookup.
    /// </summary>
    bool TryGetProduct(int id, out Product? product);

    /// <summary>
    /// Searches and retrieves a product by its ProductID key or returns null.
    /// </summary>
    Product? GetProductById(int id);

    /// <summary>
    /// Retrieves all products stored in the dictionary as a read-only list.
    /// </summary>
    IReadOnlyList<Product> GetAllProducts();

    /// <summary>
    /// Searches products matching a query against ProductID, Name, Category, or Description.
    /// </summary>
    IReadOnlyList<Product> SearchProducts(string query);

    /// <summary>
    /// Filters products by optional category, minimum price, and maximum price.
    /// </summary>
    IReadOnlyList<Product> FilterProducts(string? category, decimal? minPrice, decimal? maxPrice);

    /// <summary>
    /// Computes summary statistics across the product dictionary.
    /// </summary>
    InventoryStatistics GetStatistics();

    /// <summary>
    /// Resets the dictionary back to the default seed inventory (10+ products).
    /// </summary>
    void ResetToDefault();

    /// <summary>
    /// Removes all products from the dictionary.
    /// </summary>
    void Clear();
}
