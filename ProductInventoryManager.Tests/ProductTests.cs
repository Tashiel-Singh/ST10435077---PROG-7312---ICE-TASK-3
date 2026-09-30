using ProductInventoryManager.Core.Models;
using Xunit;

namespace ProductInventoryManager.Tests;

public class ProductTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var product = new Product(101, "Laptop", 5000m, "Computers", 15, "Workstation laptop");

        Assert.Equal(101, product.ProductID);
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(5000m, product.Price);
        Assert.Equal("Computers", product.Category);
        Assert.Equal(15, product.StockQuantity);
        Assert.Equal("Workstation laptop", product.Description);
    }

    [Fact]
    public void FormattedPrice_FormatsWithRandSymbolAndDecimals()
    {
        var product = new Product(101, "Laptop", 5000m);
        Assert.Equal("R 5,000.00", product.FormattedPrice);

        var productDecimal = new Product(102, "Speaker", 1234.50m);
        Assert.Equal("R 1,234.50", productDecimal.FormattedPrice);
    }

    [Fact]
    public void FormattedId_FormatsWithPoundPrefix()
    {
        var product = new Product(101, "Laptop", 5000m);
        Assert.Equal("#101", product.FormattedId);
    }

    [Fact]
    public void FormattedStock_FormatsWithUnitString()
    {
        var product = new Product(101, "Laptop", 5000m, "Computers", 7);
        Assert.Equal("7 unit(s)", product.FormattedStock);
    }

    [Fact]
    public void Validate_ValidProduct_ReturnsTrue()
    {
        var product = new Product(101, "Laptop", 5000m, "Computers", 10, "Test");
        var (isValid, errorMessage) = product.Validate();

        Assert.True(isValid);
        Assert.Null(errorMessage);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99)]
    public void Validate_NonPositiveId_ReturnsFalse(int invalidId)
    {
        var product = new Product(invalidId, "Laptop", 5000m);
        var (isValid, errorMessage) = product.Validate();

        Assert.False(isValid);
        Assert.Contains("positive integer", errorMessage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_BlankName_ReturnsFalse(string? invalidName)
    {
        var product = new Product(101, invalidName!, 5000m);
        var (isValid, errorMessage) = product.Validate();

        Assert.False(isValid);
        Assert.Contains("name cannot be empty", errorMessage);
    }

    [Fact]
    public void Validate_NegativePrice_ReturnsFalse()
    {
        var product = new Product(101, "Laptop", -150m);
        var (isValid, errorMessage) = product.Validate();

        Assert.False(isValid);
        Assert.Contains("price cannot be negative", errorMessage);
    }

    [Fact]
    public void Clone_CreatesIndependentDeepCopy()
    {
        var original = new Product(101, "Laptop", 5000m, "Computers", 15, "Original");
        var clone = original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(original.ProductID, clone.ProductID);
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.Price, clone.Price);
        Assert.Equal(original.Category, clone.Category);
        Assert.Equal(original.StockQuantity, clone.StockQuantity);

        // Mutating clone should not impact original
        clone.Name = "Modified";
        clone.Price = 9999m;
        Assert.Equal("Laptop", original.Name);
        Assert.Equal(5000m, original.Price);
    }

    [Fact]
    public void ToString_ContainsIdNameAndFormattedPrice()
    {
        var product = new Product(101, "Laptop", 5000m);
        string result = product.ToString();

        Assert.Contains("101", result);
        Assert.Contains("Laptop", result);
        Assert.Contains("R 5,000.00", result);
    }
}
