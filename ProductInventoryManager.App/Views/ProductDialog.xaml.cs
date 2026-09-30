using System.Windows;
using ProductInventoryManager.Core.Models;
using ProductInventoryManager.Core.Services;

namespace ProductInventoryManager.App.Views;

public partial class ProductDialog : Window
{
    private readonly IInventoryService? _inventoryService;
    private readonly bool _isEditMode;
    private readonly int _originalId;

    public Product? ResultProduct { get; private set; }

    /// <summary>
    /// Constructor for adding a new product.
    /// </summary>
    public ProductDialog(IInventoryService inventoryService)
    {
        InitializeComponent();
        _inventoryService = inventoryService;
        _isEditMode = false;

        DialogTitleTextBlock.Text = "Add New Product";
        DialogSubtitleTextBlock.Text = "Enter product details to add a new key-value pair to the Dictionary.";
        ProductIdTextBox.IsEnabled = true;

        // Suggest the next available ID
        int nextId = 101;
        while (_inventoryService.ContainsProduct(nextId))
        {
            nextId++;
        }
        ProductIdTextBox.Text = nextId.ToString();
    }

    /// <summary>
    /// Constructor for editing an existing product.
    /// </summary>
    public ProductDialog(Product existing)
    {
        InitializeComponent();
        _isEditMode = true;
        _originalId = existing.ProductID;

        DialogTitleTextBlock.Text = $"Edit Product #{existing.ProductID}";
        DialogSubtitleTextBlock.Text = "Modify product details and save updates to the Dictionary.";

        ProductIdTextBox.Text = existing.ProductID.ToString();
        ProductIdTextBox.IsEnabled = false; // Key cannot be mutated directly
        ProductNameTextBox.Text = existing.Name;
        ProductPriceTextBox.Text = existing.Price.ToString("0.00");
        ProductCategoryTextBox.Text = existing.Category;
        ProductStockTextBox.Text = existing.StockQuantity.ToString();
        ProductDescriptionTextBox.Text = existing.Description;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ErrorMessageTextBlock.Visibility = Visibility.Collapsed;

        // Validate Product ID
        if (!int.TryParse(ProductIdTextBox.Text.Trim(), out int id) || id <= 0)
        {
            ShowError("Product ID must be a positive integer greater than 0.");
            ProductIdTextBox.Focus();
            return;
        }

        if (!_isEditMode && _inventoryService != null && _inventoryService.ContainsProduct(id))
        {
            ShowError($"Product with ID {id} already exists in the dictionary! Dictionary keys must be unique.");
            ProductIdTextBox.Focus();
            return;
        }

        // Validate Name
        string name = ProductNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            ShowError("Product Name is required and cannot be empty.");
            ProductNameTextBox.Focus();
            return;
        }

        // Validate Price
        string priceRaw = ProductPriceTextBox.Text.Trim().Replace("R", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (!decimal.TryParse(priceRaw, out decimal price) || price < 0)
        {
            ShowError("Please enter a valid non-negative price (e.g., 4999.99).");
            ProductPriceTextBox.Focus();
            return;
        }

        // Validate Stock
        int stock = 1;
        if (!string.IsNullOrWhiteSpace(ProductStockTextBox.Text))
        {
            if (!int.TryParse(ProductStockTextBox.Text.Trim(), out stock) || stock < 0)
            {
                ShowError("Stock quantity must be a non-negative whole number.");
                ProductStockTextBox.Focus();
                return;
            }
        }

        string category = string.IsNullOrWhiteSpace(ProductCategoryTextBox.Text)
            ? "General"
            : ProductCategoryTextBox.Text.Trim();

        string description = ProductDescriptionTextBox.Text.Trim();

        ResultProduct = new Product(id, name, price, category, stock, description);

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void ShowError(string message)
    {
        ErrorMessageTextBlock.Text = message;
        ErrorMessageTextBlock.Visibility = Visibility.Visible;
    }
}
