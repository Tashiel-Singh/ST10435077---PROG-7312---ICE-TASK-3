using ProductInventoryManager.Core.Data;
using ProductInventoryManager.Core.Models;
using ProductInventoryManager.Core.Services;
using Xunit;

namespace ProductInventoryManager.Tests;

public class InventoryStatisticsTests
{
    [Fact]
    public void GetStatistics_CalculatesTotalValuationAndAveragesCorrectly()
    {
        var service = new DictionaryInventoryService(populateDefaults: false);
        service.AddProduct(new Product(1, "Item A", 100m, "Cat A", 2), out _); // Val: 200
        service.AddProduct(new Product(2, "Item B", 200m, "Cat B", 3), out _); // Val: 600
        service.AddProduct(new Product(3, "Item C", 300m, "Cat A", 1), out _); // Val: 300

        var stats = service.GetStatistics();

        Assert.Equal(3, stats.TotalProducts);
        Assert.Equal(6, stats.TotalStockUnits);
        Assert.Equal(1100m, stats.TotalInventoryValue);
        Assert.Equal(200m, stats.AveragePrice);
        Assert.Equal(2, stats.TotalCategories);
        Assert.Equal(3, stats.MostExpensiveProduct?.ProductID);
        Assert.Equal(1, stats.LeastExpensiveProduct?.ProductID);
        Assert.Equal("R 1,100.00", stats.FormattedTotalValue);
        Assert.Equal("R 200.00", stats.FormattedAveragePrice);
    }

    [Fact]
    public void GetStatistics_EmptyInventory_ReturnsZerosGracefully()
    {
        var service = new DictionaryInventoryService(populateDefaults: false);
        var stats = service.GetStatistics();

        Assert.Equal(0, stats.TotalProducts);
        Assert.Equal(0, stats.TotalStockUnits);
        Assert.Equal(0m, stats.TotalInventoryValue);
        Assert.Equal(0m, stats.AveragePrice);
        Assert.Null(stats.MostExpensiveProduct);
        Assert.Null(stats.LeastExpensiveProduct);
    }
}

public class SampleDataTests
{
    [Fact]
    public void GetSeedProducts_ReturnsAtLeast10Items()
    {
        var items = SampleInventoryData.GetSeedProducts();
        Assert.True(items.Count >= 10);
    }

    [Fact]
    public void GetSeedProducts_HasUniqueProductIds()
    {
        var items = SampleInventoryData.GetSeedProducts();
        var uniqueIds = items.Select(p => p.ProductID).Distinct();
        Assert.Equal(items.Count, uniqueIds.Count());
    }

    [Fact]
    public void GetSeedProducts_AllItemsPassValidation()
    {
        var items = SampleInventoryData.GetSeedProducts();
        foreach (var p in items)
        {
            var (isValid, error) = p.Validate();
            Assert.True(isValid, $"Product {p.ProductID} failed validation: {error}");
        }
    }
}
