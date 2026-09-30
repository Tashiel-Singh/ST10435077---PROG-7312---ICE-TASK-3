using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ProductInventoryManager.App.Views;
using ProductInventoryManager.Core.Models;
using ProductInventoryManager.Core.Services;

namespace ProductInventoryManager.App;

public partial class MainWindow : Window
{
    private readonly IInventoryService _inventory;
    private Product? _selectedProduct;
    private bool _isUpdatingCategories;

    public MainWindow()
    {
        InitializeComponent();
        _inventory = new DictionaryInventoryService(populateDefaults: true);

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        PopulateCategoryFilter();
        RefreshInventoryView();
    }

    private void PopulateCategoryFilter()
    {
        _isUpdatingCategories = true;

        string currentSelected = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "All Categories";

        CategoryFilterComboBox.Items.Clear();
        CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = "All Categories", IsSelected = true });

        var categories = _inventory.GetAllProducts()
            .Select(p => p.Category)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c);

        foreach (var cat in categories)
        {
            var item = new ComboBoxItem { Content = cat };
            if (string.Equals(cat, currentSelected, StringComparison.OrdinalIgnoreCase))
            {
                item.IsSelected = true;
            }
            CategoryFilterComboBox.Items.Add(item);
        }

        _isUpdatingCategories = false;
    }

    private void RefreshInventoryView()
    {
        // 1. Get filtered & searched products
        string query = SearchTextBox?.Text?.Trim() ?? string.Empty;
        var list = _inventory.SearchProducts(query);

        // Apply category filter
        string selectedCategory = (CategoryFilterComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "All Categories";
        if (!string.IsNullOrWhiteSpace(selectedCategory) && selectedCategory != "All Categories")
        {
            list = list.Where(p => string.Equals(p.Category, selectedCategory, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Apply sorting
        int sortIndex = SortComboBox?.SelectedIndex ?? 0;
        IEnumerable<Product> sorted = sortIndex switch
        {
            1 => list.OrderByDescending(p => p.ProductID),
            2 => list.OrderBy(p => p.Price),
            3 => list.OrderByDescending(p => p.Price),
            4 => list.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase),
            _ => list.OrderBy(p => p.ProductID)
        };

        var displayList = sorted.ToList();
        ProductsDataGrid.ItemsSource = displayList;

        // 2. Update Result Counter
        int totalInDict = _inventory.Count;
        ResultCountTextBlock.Text = $"Showing {displayList.Count} of {totalInDict} product(s)";

        // 3. Update KPI Metrics
        var stats = _inventory.GetStatistics();
        TotalProductsTextBlock.Text = stats.TotalProducts.ToString();
        TotalValueTextBlock.Text = stats.FormattedTotalValue;
        TotalUnitsTextBlock.Text = $"{stats.TotalStockUnits} total unit(s) in stock";
        AvgPriceTextBlock.Text = stats.FormattedAveragePrice;

        if (stats.MostExpensiveProduct != null)
        {
            TopProductTextBlock.Text = stats.MostExpensiveProduct.Name;
            TopProductPriceTextBlock.Text = $"{stats.MostExpensiveProduct.FormattedPrice} (ID: #{stats.MostExpensiveProduct.ProductID})";
        }
        else
        {
            TopProductTextBlock.Text = "None";
            TopProductPriceTextBlock.Text = "N/A";
        }

        // Re-evaluate selected item if still present
        if (_selectedProduct != null)
        {
            var match = displayList.FirstOrDefault(p => p.ProductID == _selectedProduct.ProductID);
            if (match != null)
            {
                ProductsDataGrid.SelectedItem = match;
            }
            else
            {
                ProductsDataGrid.SelectedItem = null;
                UpdateInspectorPanel(null);
            }
        }
        else
        {
            UpdateInspectorPanel(null);
        }
    }

    private void UpdateInspectorPanel(Product? product)
    {
        _selectedProduct = product;

        if (product == null)
        {
            NoSelectionPlaceholder.Visibility = Visibility.Visible;
            SelectedDetailsPanel.Visibility = Visibility.Collapsed;
            return;
        }

        NoSelectionPlaceholder.Visibility = Visibility.Collapsed;
        SelectedDetailsPanel.Visibility = Visibility.Visible;

        DetailIdTextBlock.Text = $"#{product.ProductID}";
        DetailCategoryTextBlock.Text = product.Category;
        DetailNameTextBlock.Text = product.Name;
        DetailPriceTextBlock.Text = product.FormattedPrice;
        DetailStockTextBlock.Text = $"{product.StockQuantity} unit(s) in stock";
        DetailDescriptionTextBlock.Text = string.IsNullOrWhiteSpace(product.Description)
            ? "No additional description provided for this product."
            : product.Description;

        // Diagnostic information about Dictionary
        DiagKeyTextBlock.Text = $"{product.ProductID} (int)";
        DiagHashTextBlock.Text = product.ProductID.GetHashCode().ToString();
    }

    private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductsDataGrid.SelectedItem is Product product)
        {
            UpdateInspectorPanel(product);
        }
        else
        {
            UpdateInspectorPanel(null);
        }
    }

    private void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ProductDialog(_inventory)
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true && dialog.ResultProduct != null)
        {
            if (_inventory.AddProduct(dialog.ResultProduct, out string error))
            {
                ShowToast("✓", $"Product #{dialog.ResultProduct.ProductID} ('{dialog.ResultProduct.Name}') added to Dictionary!", "#10B981");
                PopulateCategoryFilter();
                RefreshInventoryView();

                // Select the newly added item
                var newlyAdded = _inventory.GetProductById(dialog.ResultProduct.ProductID);
                if (newlyAdded != null)
                {
                    ProductsDataGrid.SelectedItem = newlyAdded;
                    ProductsDataGrid.ScrollIntoView(newlyAdded);
                }
            }
            else
            {
                MessageBox.Show($"Failed to add product: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ResetSeedButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to reset the inventory back to the 12 default products?",
            "Reset Seed Data",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _inventory.ResetToDefault();
            ShowToast("🔄", "Dictionary reset back to 12 default assignment seed products.", "#38BDF8");
            PopulateCategoryFilter();
            RefreshInventoryView();
        }
    }

    private void OpenCliButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            string baseDir = AppContext.BaseDirectory;
            // Locate ProductInventoryManager.Cli project folder
            string cliProject = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "ProductInventoryManager.Cli"));

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{cliProject}\"",
                UseShellExecute = true
            };

            Process.Start(psi);
            ShowToast("💻", "Interactive Console CLI launched in an external terminal window.", "#818CF8");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not launch CLI: {ex.Message}\nRun via: dotnet run --project ProductInventoryManager.Cli",
                "Console Launch", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshInventoryView();
    }

    private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isUpdatingCategories) return;
        RefreshInventoryView();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        RefreshInventoryView();
    }

    private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
    {
        SearchTextBox.Text = string.Empty;
        if (CategoryFilterComboBox.Items.Count > 0)
        {
            CategoryFilterComboBox.SelectedIndex = 0;
        }
        SortComboBox.SelectedIndex = 0;
        RefreshInventoryView();
        ShowToast("🧹", "All filters and search criteria cleared.", "#94A3B8");
    }

    private void RowEditButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.DataContext is Product product)
        {
            EditProduct(product);
        }
    }

    private void RowDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.DataContext is Product product)
        {
            DeleteProduct(product);
        }
    }

    private void DetailEditButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedProduct != null)
        {
            EditProduct(_selectedProduct);
        }
    }

    private void DetailDeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedProduct != null)
        {
            DeleteProduct(_selectedProduct);
        }
    }

    private void EditProduct(Product product)
    {
        var dialog = new ProductDialog(product)
        {
            Owner = this
        };

        if (dialog.ShowDialog() == true && dialog.ResultProduct != null)
        {
            var res = dialog.ResultProduct;
            if (_inventory.UpdateProduct(res.ProductID, res.Name, res.Price, res.Category, res.StockQuantity, res.Description, out string error))
            {
                ShowToast("✓", $"Product #{res.ProductID} ('{res.Name}') successfully updated in Dictionary.", "#10B981");
                PopulateCategoryFilter();
                RefreshInventoryView();
            }
            else
            {
                MessageBox.Show($"Failed to update product: {error}", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void DeleteProduct(Product product)
    {
        var confirm = new ConfirmDialog(product.Name, product.ProductID, product.FormattedPrice)
        {
            Owner = this
        };

        if (confirm.ShowDialog() == true)
        {
            if (_inventory.DeleteProduct(product.ProductID, out string error))
            {
                ShowToast("🗑️", $"Product #{product.ProductID} removed from Dictionary. Remaining: {_inventory.Count}.", "#F43F5E");
                PopulateCategoryFilter();
                RefreshInventoryView();
            }
            else
            {
                MessageBox.Show($"Failed to delete product: {error}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ShowToast(string icon, string message, string hexColor)
    {
        ToastIconTextBlock.Text = icon;
        ToastMessageTextBlock.Text = message;
        try
        {
            var color = (Color)ColorConverter.ConvertFromString(hexColor);
            ToastMessageTextBlock.Foreground = new SolidColorBrush(color);
        }
        catch
        {
            ToastMessageTextBlock.Foreground = new SolidColorBrush(Colors.LightGray);
        }
    }
}