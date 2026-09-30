using ProductInventoryManager.Core.Models;
using ProductInventoryManager.Core.Services;
using Xunit;

namespace ProductInventoryManager.Tests;

public class InventoryServiceTests
{
    [Fact]
    public void Constructor_DefaultSeed_ContainsAtLeast10Products()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        Assert.True(service.Count >= 10);
    }

    [Fact]
    public void SeedData_ContainsAssignmentRequiredProducts()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        // Required by assignment:
        // ProductID = 101, Name = "Laptop", Price = R5000
        // ProductID = 102, Name = "JBL Speaker", Price = R8000
        // ProductID = 103, Name = "Tablet", Price = R2300

        Assert.True(service.TryGetProduct(101, out var p101));
        Assert.NotNull(p101);
        Assert.Equal("Laptop", p101.Name);
        Assert.Equal(5000m, p101.Price);

        Assert.True(service.TryGetProduct(102, out var p102));
        Assert.NotNull(p102);
        Assert.Equal("JBL Speaker", p102.Name);
        Assert.Equal(8000m, p102.Price);

        Assert.True(service.TryGetProduct(103, out var p103));
        Assert.NotNull(p103);
        Assert.Equal("Tablet", p103.Name);
        Assert.Equal(2300m, p103.Price);
    }

    [Fact]
    public void AddProduct_ValidProduct_ReturnsTrueAndAddsToDictionary()
    {
        var service = new DictionaryInventoryService(populateDefaults: false);
        var product = new Product(201, "Smartwatch", 2500m, "Wearables", 5);

        bool result = service.AddProduct(product, out string error);

        Assert.True(result);
        Assert.Empty(error);
        Assert.Equal(1, service.Count);
        Assert.True(service.ContainsProduct(201));
    }

    [Fact]
    public void AddProduct_DuplicateId_ReturnsFalseAndRejects()
    {
        var service = new DictionaryInventoryService(populateDefaults: false);
        var p1 = new Product(101, "Laptop", 5000m);
        var p2 = new Product(101, "Another Laptop", 6000m);

        service.AddProduct(p1, out _);
        bool result = service.AddProduct(p2, out string error);

        Assert.False(result);
        Assert.Contains("already exists", error);
        Assert.Equal(1, service.Count);
    }

    [Fact]
    public void AddProduct_NullProduct_ReturnsFalse()
    {
        var service = new DictionaryInventoryService(populateDefaults: false);
        bool result = service.AddProduct(null!, out string error);

        Assert.False(result);
        Assert.Contains("null", error);
    }

    [Fact]
    public void AddProduct_InvalidData_ReturnsFalse()
    {
        var service = new DictionaryInventoryService(populateDefaults: false);
        var invalidProduct = new Product(-5, "", -100m);

        bool result = service.AddProduct(invalidProduct, out string error);

        Assert.False(result);
        Assert.NotEmpty(error);
    }

    [Fact]
    public void TryGetProduct_ExistingId_ReturnsTrueAndProduct()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        bool found = service.TryGetProduct(101, out var product);

        Assert.True(found);
        Assert.NotNull(product);
        Assert.Equal(101, product.ProductID);
        Assert.Equal("Laptop", product.Name);
    }

    [Fact]
    public void TryGetProduct_NonExistingId_ReturnsFalseAndNull()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        bool found = service.TryGetProduct(9999, out var product);

        Assert.False(found);
        Assert.Null(product);
    }

    [Fact]
    public void GetProductById_ReturnsCorrectEntityOrNull()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        var p = service.GetProductById(102);
        Assert.NotNull(p);
        Assert.Equal("JBL Speaker", p.Name);

        var missing = service.GetProductById(8888);
        Assert.Null(missing);
    }

    [Fact]
    public void GetAllProducts_ReturnsAllDictionaryValues()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);
        var all = service.GetAllProducts();

        Assert.Equal(service.Count, all.Count);
        // Verify sorted order by ProductID
        var sorted = all.OrderBy(p => p.ProductID).ToList();
        Assert.Equal(sorted.Select(p => p.ProductID), all.Select(p => p.ProductID));
    }

    [Fact]
    public void UpdateProduct_ExistingProduct_UpdatesDetailsSuccessfully()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        bool result = service.UpdateProduct(101, "Ultra Laptop Pro", 7500m, "Laptops", 20, "Updated desc", out string error);

        Assert.True(result);
        Assert.Empty(error);

        var updated = service.GetProductById(101);
        Assert.NotNull(updated);
        Assert.Equal("Ultra Laptop Pro", updated.Name);
        Assert.Equal(7500m, updated.Price);
        Assert.Equal("Laptops", updated.Category);
        Assert.Equal(20, updated.StockQuantity);
        Assert.Equal("Updated desc", updated.Description);
    }

    [Fact]
    public void UpdateProduct_NonExistingId_ReturnsFalse()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        bool result = service.UpdateProduct(9999, "NonExistent", 100m, null, null, null, out string error);

        Assert.False(result);
        Assert.Contains("does not exist", error);
    }

    [Fact]
    public void UpdateProduct_InvalidParameters_ReturnsFalse()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        // Blank name
        bool res1 = service.UpdateProduct(101, "", 100m, null, null, null, out string err1);
        Assert.False(res1);
        Assert.Contains("name cannot be empty", err1);

        // Negative price
        bool res2 = service.UpdateProduct(101, "Laptop", -50m, null, null, null, out string err2);
        Assert.False(res2);
        Assert.Contains("price cannot be negative", err2);

        // Negative stock
        bool res3 = service.UpdateProduct(101, "Laptop", 100m, null, -5, null, out string err3);
        Assert.False(res3);
        Assert.Contains("Stock quantity cannot be negative", err3);
    }

    [Fact]
    public void DeleteProduct_ExistingId_RemovesFromDictionary()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);
        int initialCount = service.Count;

        bool result = service.DeleteProduct(103, out string error);

        Assert.True(result);
        Assert.Empty(error);
        Assert.Equal(initialCount - 1, service.Count);
        Assert.False(service.ContainsProduct(103));
    }

    [Fact]
    public void DeleteProduct_NonExistingId_ReturnsFalse()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);
        int initialCount = service.Count;

        bool result = service.DeleteProduct(7777, out string error);

        Assert.False(result);
        Assert.Contains("not found", error);
        Assert.Equal(initialCount, service.Count);
    }

    [Fact]
    public void SearchProducts_MatchesIdAndName()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        // Search by exact ID
        var byId = service.SearchProducts("102");
        Assert.Contains(byId, p => p.ProductID == 102);

        // Search by Name substring (case-insensitive)
        var byName = service.SearchProducts("speaker");
        Assert.Contains(byName, p => p.ProductID == 102);

        // Search by Category
        var byCategory = service.SearchProducts("Audio");
        Assert.Contains(byCategory, p => p.Name.Contains("Speaker") || p.Name.Contains("Headphones"));
    }

    [Fact]
    public void SearchProducts_EmptyQuery_ReturnsAll()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);
        var results = service.SearchProducts("");

        Assert.Equal(service.Count, results.Count);
    }

    [Fact]
    public void FilterProducts_ByCategoryAndPrice()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);

        // Filter Audio
        var audioOnly = service.FilterProducts("Audio", null, null);
        Assert.All(audioOnly, p => Assert.Equal("Audio", p.Category));

        // Filter Price >= 5000
        var expensive = service.FilterProducts(null, 5000m, null);
        Assert.All(expensive, p => Assert.True(p.Price >= 5000m));
    }

    [Fact]
    public void ResetToDefault_RestoresSeedInventory()
    {
        var service = new DictionaryInventoryService(populateDefaults: true);
        service.Clear();
        Assert.Equal(0, service.Count);

        service.ResetToDefault();
        Assert.True(service.Count >= 10);
        Assert.True(service.ContainsProduct(101));
    }
}
