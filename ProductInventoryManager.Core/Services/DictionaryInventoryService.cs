using ProductInventoryManager.Core.Data;
using ProductInventoryManager.Core.Models;

namespace ProductInventoryManager.Core.Services;

/// <summary>
/// Core implementation of the Product Inventory System using C#'s Dictionary&lt;int, Product&gt;
/// where the key (int) is the ProductID and the value is the Product object.
/// Provides thread-safe, robust CRUD operations fulfilling all assignment requirements.
/// </summary>
public class DictionaryInventoryService : IInventoryService
{
    private readonly Dictionary<int, Product> _inventory = new();
    private readonly object _lock = new();

    public DictionaryInventoryService(bool populateDefaults = true)
    {
        if (populateDefaults)
        {
            ResetToDefault();
        }
    }

    /// <inheritdoc />
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _inventory.Count;
            }
        }
    }

    /// <inheritdoc />
    public bool ContainsProduct(int id)
    {
        lock (_lock)
        {
            return _inventory.ContainsKey(id);
        }
    }

    /// <inheritdoc />
    public bool AddProduct(Product product, out string errorMessage)
    {
        if (product == null)
        {
            errorMessage = "Product data cannot be null.";
            return false;
        }

        var (isValid, validationError) = product.Validate();
        if (!isValid)
        {
            errorMessage = validationError ?? "Invalid product data.";
            return false;
        }

        lock (_lock)
        {
            if (_inventory.ContainsKey(product.ProductID))
            {
                errorMessage = $"Product with ID {product.ProductID} already exists in the inventory.";
                return false;
            }

            _inventory.Add(product.ProductID, product.Clone());
            errorMessage = string.Empty;
            return true;
        }
    }

    /// <inheritdoc />
    public bool UpdateProduct(
        int id,
        string newName,
        decimal newPrice,
        string? newCategory,
        int? newStockQuantity,
        string? newDescription,
        out string errorMessage)
    {
        if (id <= 0)
        {
            errorMessage = "Invalid Product ID. Must be greater than 0.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            errorMessage = "Product name cannot be empty or whitespace.";
            return false;
        }

        if (newPrice < 0)
        {
            errorMessage = "Product price cannot be negative.";
            return false;
        }

        if (newStockQuantity.HasValue && newStockQuantity.Value < 0)
        {
            errorMessage = "Stock quantity cannot be negative.";
            return false;
        }

        lock (_lock)
        {
            if (!_inventory.TryGetValue(id, out var product))
            {
                errorMessage = $"Product with ID {id} does not exist in the inventory.";
                return false;
            }

            product.Name = newName.Trim();
            product.Price = newPrice;
            if (!string.IsNullOrWhiteSpace(newCategory))
            {
                product.Category = newCategory.Trim();
            }
            if (newStockQuantity.HasValue)
            {
                product.StockQuantity = newStockQuantity.Value;
            }
            if (newDescription != null)
            {
                product.Description = newDescription.Trim();
            }

            errorMessage = string.Empty;
            return true;
        }
    }

    /// <inheritdoc />
    public bool DeleteProduct(int id, out string errorMessage)
    {
        lock (_lock)
        {
            if (_inventory.Remove(id))
            {
                errorMessage = string.Empty;
                return true;
            }

            errorMessage = $"Product with ID {id} was not found in the inventory.";
            return false;
        }
    }

    /// <inheritdoc />
    public bool TryGetProduct(int id, out Product? product)
    {
        lock (_lock)
        {
            if (_inventory.TryGetValue(id, out var found))
            {
                product = found.Clone();
                return true;
            }

            product = null;
            return false;
        }
    }

    /// <inheritdoc />
    public Product? GetProductById(int id)
    {
        TryGetProduct(id, out var product);
        return product;
    }

    /// <inheritdoc />
    public IReadOnlyList<Product> GetAllProducts()
    {
        lock (_lock)
        {
            return _inventory.Values
                .Select(p => p.Clone())
                .OrderBy(p => p.ProductID)
                .ToList();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Product> SearchProducts(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllProducts();
        }

        string trimmed = query.Trim();
        bool isNumber = int.TryParse(trimmed, out int searchId);

        lock (_lock)
        {
            return _inventory.Values
                .Where(p =>
                    (isNumber && p.ProductID == searchId) ||
                    p.ProductID.ToString().Contains(trimmed) ||
                    p.Name.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ||
                    p.Category.Contains(trimmed, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(trimmed, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Clone())
                .OrderBy(p => p.ProductID)
                .ToList();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<Product> FilterProducts(string? category, decimal? minPrice, decimal? maxPrice)
    {
        lock (_lock)
        {
            var query = _inventory.Values.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(category) && !category.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            return query
                .Select(p => p.Clone())
                .OrderBy(p => p.ProductID)
                .ToList();
        }
    }

    /// <inheritdoc />
    public InventoryStatistics GetStatistics()
    {
        lock (_lock)
        {
            if (_inventory.Count == 0)
            {
                return new InventoryStatistics();
            }

            return new InventoryStatistics
            {
                TotalProducts = _inventory.Count,
                TotalStockUnits = _inventory.Values.Sum(p => p.StockQuantity),
                TotalInventoryValue = _inventory.Values.Sum(p => p.Price * p.StockQuantity),
                AveragePrice = _inventory.Values.Average(p => p.Price),
                MostExpensiveProduct = _inventory.Values.MaxBy(p => p.Price)?.Clone(),
                LeastExpensiveProduct = _inventory.Values.MinBy(p => p.Price)?.Clone(),
                TotalCategories = _inventory.Values.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).Count()
            };
        }
    }

    /// <inheritdoc />
    public void ResetToDefault()
    {
        lock (_lock)
        {
            _inventory.Clear();
            foreach (var product in SampleInventoryData.GetSeedProducts())
            {
                _inventory.Add(product.ProductID, product.Clone());
            }
        }
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (_lock)
        {
            _inventory.Clear();
        }
    }
}
