using System.Diagnostics;
using ProductInventoryManager.Core.Models;
using ProductInventoryManager.Core.Services;

namespace ProductInventoryManager.Cli;

internal class Program
{
    private static readonly IInventoryService _inventory = new DictionaryInventoryService();

    private static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Product Inventory Manager — C# Dictionary Application";

        bool exitRequested = false;
        while (!exitRequested)
        {
            PrintHeader();
            PrintMainMenu();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nSelect an option [0-9]: ");
            Console.ResetColor();

            string? choice = Console.ReadLine()?.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    DisplayAllProducts();
                    break;
                case "2":
                    AddNewProduct();
                    break;
                case "3":
                    SearchProductById();
                    break;
                case "4":
                    SearchProductsByKeyword();
                    break;
                case "5":
                    UpdateProduct();
                    break;
                case "6":
                    DeleteProduct();
                    break;
                case "7":
                    DisplayInventoryStatistics();
                    break;
                case "8":
                    ResetInventory();
                    break;
                case "9":
                    LaunchWpfGui();
                    break;
                case "0":
                    exitRequested = true;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Thank you for using Product Inventory Manager. Goodbye!");
                    Console.ResetColor();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid option. Please enter a number between 0 and 9.");
                    Console.ResetColor();
                    Pause();
                    break;
            }
        }
    }

    private static void PrintHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================================================================");
        Console.WriteLine("       🛒 PRODUCT INVENTORY MANAGER — C# DICTIONARY APPLICATION                 ");
        Console.WriteLine("            PROG7312 Activity | Data Structure: Dictionary<int, Product>        ");
        Console.WriteLine("================================================================================");
        Console.ResetColor();
    }

    private static void PrintMainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Current Inventory Count: {_inventory.Count} product(s) stored in Dictionary\n");
        Console.ResetColor();

        Console.WriteLine("  [1] 📋 Display All Products");
        Console.WriteLine("  [2] ➕ Add New Product");
        Console.WriteLine("  [3] 🔍 Search Product by ProductID (O(1) Lookup)");
        Console.WriteLine("  [4] 🔎 Search Products by Name / Keyword");
        Console.WriteLine("  [5] ✏️  Update Product Details");
        Console.WriteLine("  [6] 🗑️  Delete Product by ProductID");
        Console.WriteLine("  [7] 📊 View Inventory Analytics & Statistics");
        Console.WriteLine("  [8] 🔄 Reset to Default 12 Seed Products");
        Console.WriteLine("  [9] 🖥️  Launch Modern WPF GUI Application");
        Console.WriteLine("  [0] 🚪 Exit Application");
    }

    private static void DisplayAllProducts()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [1] ALL PRODUCTS IN DICTIONARY ---");
        Console.ResetColor();

        var products = _inventory.GetAllProducts();
        if (products.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The dictionary is currently empty. Add products using option [2] or reset using [8].");
            Console.ResetColor();
            Pause();
            return;
        }

        PrintProductTable(products);
        Pause();
    }

    private static void AddNewProduct()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [2] ADD NEW PRODUCT ---");
        Console.ResetColor();

        int id;
        while (true)
        {
            Console.Write("Enter ProductID (positive integer, e.g. 115): ");
            string? idInput = Console.ReadLine();
            if (int.TryParse(idInput, out id) && id > 0)
            {
                if (_inventory.ContainsProduct(id))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: ProductID {id} already exists in the dictionary! Key must be unique.");
                    Console.ResetColor();
                    continue;
                }
                break;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid ID. Please enter a valid positive whole number.");
            Console.ResetColor();
        }

        string name;
        while (true)
        {
            Console.Write("Enter Product Name (e.g., Wireless Earbuds): ");
            name = Console.ReadLine()?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                break;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Product Name cannot be blank.");
            Console.ResetColor();
        }

        decimal price;
        while (true)
        {
            Console.Write("Enter Price in Rands (e.g., 899.99): R ");
            string? priceInput = Console.ReadLine();
            if (decimal.TryParse(priceInput, out price) && price >= 0)
            {
                break;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid price. Please enter a non-negative decimal number.");
            Console.ResetColor();
        }

        Console.Write("Enter Category (optional, press Enter for 'General'): ");
        string? categoryInput = Console.ReadLine();
        string category = string.IsNullOrWhiteSpace(categoryInput) ? "General" : categoryInput.Trim();

        int stock = 1;
        Console.Write("Enter Stock Quantity (optional, default 1): ");
        string? stockInput = Console.ReadLine();
        if (int.TryParse(stockInput, out int parsedStock) && parsedStock >= 0)
        {
            stock = parsedStock;
        }

        Console.Write("Enter Product Description (optional): ");
        string description = Console.ReadLine()?.Trim() ?? string.Empty;

        var product = new Product(id, name, price, category, stock, description);
        if (_inventory.AddProduct(product, out string error))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ Success! Product #{id} ('{name}') added to Dictionary. Total items: {_inventory.Count}.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n✗ Failed to add product: {error}");
            Console.ResetColor();
        }

        Pause();
    }

    private static void SearchProductById()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [3] SEARCH PRODUCT BY ID (O(1) DICTIONARY LOOKUP) ---");
        Console.ResetColor();

        Console.Write("Enter ProductID to search: ");
        if (!int.TryParse(Console.ReadLine(), out int searchId))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid ID entered.");
            Console.ResetColor();
            Pause();
            return;
        }

        if (_inventory.TryGetProduct(searchId, out var product) && product != null)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ Product found in Dictionary<int, Product>!");
            Console.ResetColor();

            Console.WriteLine($"  ----------------------------------------------");
            Console.WriteLine($"  ProductID:   {product.ProductID}");
            Console.WriteLine($"  Name:        {product.Name}");
            Console.WriteLine($"  Price:       {product.FormattedPrice}");
            Console.WriteLine($"  Category:    {product.Category}");
            Console.WriteLine($"  Stock:       {product.StockQuantity} unit(s)");
            Console.WriteLine($"  Description: {product.Description}");
            Console.WriteLine($"  Date Added:  {product.DateAdded:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"  ----------------------------------------------");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n✗ No product with ProductID = {searchId} exists in the dictionary.");
            Console.ResetColor();
        }

        Pause();
    }

    private static void SearchProductsByKeyword()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [4] SEARCH PRODUCTS BY KEYWORD ---");
        Console.ResetColor();

        Console.Write("Enter keyword (matches Name, Category, or Description): ");
        string keyword = Console.ReadLine()?.Trim() ?? string.Empty;

        var results = _inventory.SearchProducts(keyword);
        if (results.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nNo products found matching '{keyword}'.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nFound {results.Count} matching product(s):");
            Console.ResetColor();
            PrintProductTable(results);
        }

        Pause();
    }

    private static void UpdateProduct()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [5] UPDATE PRODUCT DETAILS ---");
        Console.ResetColor();

        Console.Write("Enter ProductID of the product to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid ID.");
            Console.ResetColor();
            Pause();
            return;
        }

        if (!_inventory.TryGetProduct(id, out var existing) || existing == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Product with ID {id} does not exist in the dictionary.");
            Console.ResetColor();
            Pause();
            return;
        }

        Console.WriteLine("\nCurrent Details:");
        Console.WriteLine($"  Name:        {existing.Name}");
        Console.WriteLine($"  Price:       {existing.FormattedPrice}");
        Console.WriteLine($"  Category:    {existing.Category}");
        Console.WriteLine($"  Stock:       {existing.StockQuantity}");
        Console.WriteLine($"  Description: {existing.Description}\n");

        Console.Write($"Enter New Name (press Enter to keep '{existing.Name}'): ");
        string? newNameInput = Console.ReadLine();
        string newName = string.IsNullOrWhiteSpace(newNameInput) ? existing.Name : newNameInput.Trim();

        Console.Write($"Enter New Price (press Enter to keep '{existing.FormattedPrice}'): R ");
        string? newPriceInput = Console.ReadLine();
        decimal newPrice = existing.Price;
        if (!string.IsNullOrWhiteSpace(newPriceInput))
        {
            if (!decimal.TryParse(newPriceInput, out newPrice) || newPrice < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid price entered. Keeping existing price.");
                Console.ResetColor();
                newPrice = existing.Price;
            }
        }

        Console.Write($"Enter New Category (press Enter to keep '{existing.Category}'): ");
        string? newCatInput = Console.ReadLine();
        string newCat = string.IsNullOrWhiteSpace(newCatInput) ? existing.Category : newCatInput.Trim();

        Console.Write($"Enter New Stock (press Enter to keep '{existing.StockQuantity}'): ");
        string? newStockInput = Console.ReadLine();
        int newStock = existing.StockQuantity;
        if (!string.IsNullOrWhiteSpace(newStockInput))
        {
            if (!int.TryParse(newStockInput, out newStock) || newStock < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid stock entered. Keeping existing stock.");
                Console.ResetColor();
                newStock = existing.StockQuantity;
            }
        }

        Console.Write($"Enter New Description (press Enter to keep existing): ");
        string? newDescInput = Console.ReadLine();
        string newDesc = string.IsNullOrWhiteSpace(newDescInput) ? existing.Description : newDescInput.Trim();

        if (_inventory.UpdateProduct(id, newName, newPrice, newCat, newStock, newDesc, out string error))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ Success! Product #{id} was successfully updated in the dictionary.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n✗ Failed to update product: {error}");
            Console.ResetColor();
        }

        Pause();
    }

    private static void DeleteProduct()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [6] DELETE PRODUCT BY ID ---");
        Console.ResetColor();

        Console.Write("Enter ProductID of the product to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid ID.");
            Console.ResetColor();
            Pause();
            return;
        }

        if (!_inventory.TryGetProduct(id, out var product) || product == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Product with ID {id} does not exist in the dictionary.");
            Console.ResetColor();
            Pause();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Are you sure you want to delete Product #{id}: '{product.Name}' ({product.FormattedPrice})?");
        Console.Write("Type 'Y' to confirm deletion, or any other key to cancel: ");
        Console.ResetColor();

        string? confirmation = Console.ReadLine()?.Trim().ToUpperInvariant();
        if (confirmation == "Y")
        {
            if (_inventory.DeleteProduct(id, out string error))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✓ Product #{id} was removed from the dictionary. Remaining items: {_inventory.Count}.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n✗ Failed to delete product: {error}");
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine("\nDeletion cancelled by user.");
        }

        Pause();
    }

    private static void DisplayInventoryStatistics()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("--- [7] INVENTORY ANALYTICS & STATISTICS ---");
        Console.ResetColor();

        var stats = _inventory.GetStatistics();

        Console.WriteLine($"  Total Distinct Products: {stats.TotalProducts}");
        Console.WriteLine($"  Total Stock Units:       {stats.TotalStockUnits}");
        Console.WriteLine($"  Total Inventory Value:   {stats.FormattedTotalValue}");
        Console.WriteLine($"  Average Product Price:   {stats.FormattedAveragePrice}");
        Console.WriteLine($"  Distinct Categories:     {stats.TotalCategories}");

        if (stats.MostExpensiveProduct != null)
        {
            Console.WriteLine($"  Most Expensive Item:     #{stats.MostExpensiveProduct.ProductID} - {stats.MostExpensiveProduct.Name} ({stats.MostExpensiveProduct.FormattedPrice})");
        }

        if (stats.LeastExpensiveProduct != null)
        {
            Console.WriteLine($"  Least Expensive Item:    #{stats.LeastExpensiveProduct.ProductID} - {stats.LeastExpensiveProduct.Name} ({stats.LeastExpensiveProduct.FormattedPrice})");
        }

        Pause();
    }

    private static void ResetInventory()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Reset dictionary back to default seed products (12 items)? [y/N]: ");
        Console.ResetColor();

        string? confirm = Console.ReadLine()?.Trim().ToUpperInvariant();
        if (confirm == "Y")
        {
            _inventory.ResetToDefault();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n✓ Inventory reset successfully. Total items in dictionary: {_inventory.Count}.");
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("Reset aborted.");
        }

        Pause();
    }

    private static void LaunchWpfGui()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Launching ProductInventoryManager.App (WPF GUI)...");
        Console.ResetColor();

        try
        {
            string appProject = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "ProductInventoryManager.App"));
            var processInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{appProject}\"",
                UseShellExecute = true
            };
            Process.Start(processInfo);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("WPF GUI launched in a separate window.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Could not launch GUI directly: {ex.Message}");
            Console.WriteLine("You can run it via: dotnet run --project ProductInventoryManager.App");
            Console.ResetColor();
        }

        Pause();
    }

    private static void PrintProductTable(IReadOnlyList<Product> products)
    {
        Console.WriteLine("+-----+------------------------------------+---------------+------------+-------+");
        Console.WriteLine("| ID  | Name                               | Category      | Price      | Stock |");
        Console.WriteLine("+-----+------------------------------------+---------------+------------+-------+");

        foreach (var p in products)
        {
            string name = p.Name.Length > 34 ? p.Name.Substring(0, 31) + "..." : p.Name;
            string category = p.Category.Length > 13 ? p.Category.Substring(0, 10) + "..." : p.Category;
            Console.WriteLine($"| {p.ProductID,-3} | {name,-34} | {category,-13} | {p.FormattedPrice,10} | {p.StockQuantity,5} |");
        }

        Console.WriteLine("+-----+------------------------------------+---------------+------------+-------+");
        Console.WriteLine($"Total: {products.Count} product(s)\n");
    }

    private static void Pause()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("\nPress [Enter] to continue...");
        Console.ResetColor();
        Console.ReadLine();
    }
}
