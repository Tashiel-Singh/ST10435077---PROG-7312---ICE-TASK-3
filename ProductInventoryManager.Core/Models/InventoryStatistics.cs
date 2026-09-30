namespace ProductInventoryManager.Core.Models;

/// <summary>
/// Aggregates analytical metrics calculated across the entire product dictionary.
/// </summary>
public class InventoryStatistics
{
    public int TotalProducts { get; set; }
    public int TotalStockUnits { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public decimal AveragePrice { get; set; }
    public Product? MostExpensiveProduct { get; set; }
    public Product? LeastExpensiveProduct { get; set; }
    public int TotalCategories { get; set; }

    public string FormattedTotalValue => $"R {TotalInventoryValue.ToString("N2", System.Globalization.CultureInfo.InvariantCulture)}";
    public string FormattedAveragePrice => $"R {AveragePrice.ToString("N2", System.Globalization.CultureInfo.InvariantCulture)}";
}
