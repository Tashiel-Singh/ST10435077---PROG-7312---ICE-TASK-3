# 🛒 Product Inventory Manager – C# Dictionary Application

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![WPF GUI](https://img.shields.io/badge/GUI-WPF-0078D4?logo=windows&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/)
[![Tests](https://img.shields.io/badge/Tests-38%20Passed%20(100%25)-brightgreen?logo=xunit)](file:///ProductInventoryManager.Tests)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

> **Course:** PROG7312 — Advanced Programming  
> **Activity:** ICE Task 3 — Product Inventory Manager  
> **Student Name:** Tashiel Singh  
> **Student Number:** ST10435077  
> **GitHub Submission Link:** [https://classroom.github.com/a/6J_G2A_A](https://classroom50.org/EMKNDN/emkndn-prog7312-g2-2026/assignments/prog7312-ice-task-3/accept)  

---

## 📘 Scenario & Objective

You are tasked with building a robust **Product Inventory System** using the `Dictionary<int, Product>` data structure in C#. This system manages products using their unique **ProductID** (`int`) as the hash key and stores product details (**Name**, **Price**, **Category**, **StockQuantity**, and **Description**) as values.

The system features:
1. **Core Data Structure:** Pure `Dictionary<int, Product>` collection ensuring $O(1)$ constant-time lookup, insertion, update, and deletion.
2. **Interactive Console CLI (`ProductInventoryManager.Cli`):** Prompt-driven terminal interface with colored formatting and structured tables.
3. **Modern WPF GUI (`ProductInventoryManager.App`):** Real-time reactive dashboard with dark obsidian glassmorphic styling, instant live search, category filtration, KPI analytics cards, modal dialogs, and diagnostic inspector.
4. **Automated Test Suite (`ProductInventoryManager.Tests`):** 38 unit tests covering 100% of domain and service operations with sub-50ms execution.

---

## 🎯 Learning Goals & Implementation Matrix

| Requirement | Description | Implementation Detail | Location |
| :--- | :--- | :--- | :--- |
| **Dictionary Structure** | Define `Dictionary<int, Product>` with `int` as ProductID and `Product` class/struct. | Implemented `DictionaryInventoryService` holding `Dictionary<int, Product>` with thread-safe locking and immutability guards. | [`DictionaryInventoryService.cs`](file:///ProductInventoryManager.Core/Services/DictionaryInventoryService.cs) |
| **Initial Seed Data** | Add at least 10 products, including 101 Laptop (R5000), 102 JBL Speaker (R8000), 103 Tablet (R2300). | Initialized with 12 seed products across Computers, Audio, Displays, and Peripherals. | [`SampleInventoryData.cs`](file:///ProductInventoryManager.Core/Data/SampleInventoryData.cs) |
| **Add Operation** | Add new product, validate positive ID, uniqueness, non-empty name, non-negative price. | `AddProduct()` checks `ContainsKey(id)` and validates business constraints before insertion. | [`DictionaryInventoryService.cs#L45-L75`](file:///ProductInventoryManager.Core/Services/DictionaryInventoryService.cs) |
| **Update Operation** | Modify existing product details by ProductID key. | `UpdateProduct()` validates key presence via `TryGetValue()` and mutates fields safely. | [`DictionaryInventoryService.cs#L77-L125`](file:///ProductInventoryManager.Core/Services/DictionaryInventoryService.cs) |
| **Search Operation** | Instant $O(1)$ key lookup and flexible multi-field keyword filtering. | `TryGetProduct()` for $O(1)$ ID search; `SearchProducts()` for Name/Category/Description matching. | [`DictionaryInventoryService.cs#L145-L210`](file:///ProductInventoryManager.Core/Services/DictionaryInventoryService.cs) |
| **Delete Operation** | Remove product from dictionary by ProductID key. | `DeleteProduct()` calls `_inventory.Remove(id)` with graceful error reporting. | [`DictionaryInventoryService.cs#L127-L143`](file:///ProductInventoryManager.Core/Services/DictionaryInventoryService.cs) |
| **Display Operation** | Display all products formatted neatly with currency (ZAR `R`). | Formatted display in CLI table and WPF DataGrid with invariant currency formatting (`R 5,000.00`). | [`Product.cs#L22`](file:///ProductInventoryManager.Core/Models/Product.cs) |
| **User Interaction** | Practice interaction via console prompts. | Full-featured CLI prompt menu with input loops, validations, and interactive dialogs. | [`Program.cs`](file:///ProductInventoryManager.Cli/Program.cs) |
| **GUI Enhancement** | Enhance with a GUI for real-time display and interaction. | Rich WPF Application with KPI cards, DataGrid, search/filters, modal add/edit dialog, delete confirmation, and dictionary diagnostics. | [`MainWindow.xaml`](file:///ProductInventoryManager.App/MainWindow.xaml) |

---

## 💡 C# Dictionary Deep-Dive: Why `Dictionary<int, Product>`?

A `Dictionary<TKey, TValue>` in C# is a generic hash table implementation offering significant performance advantages over traditional arrays or linked lists:

```
                  +---------------------------+
  ProductID (int) | Hash Function:            |
  Key: 102 -----> | GetHashCode() -> Bucket # | -----> O(1) Constant Lookup
                  +---------------------------+
                                |
                                v
               [Bucket Table] -> Entry { Key = 102, Value = Product("JBL Speaker", R8000) }
```

### Algorithmic Complexity Comparison

| Operation | `List<Product>` | `LinkedList<Product>` | `Dictionary<int, Product>` (Our Choice) |
| :--- | :---: | :---: | :---: |
| **Search by ID** | $O(N)$ Linear | $O(N)$ Linear | **$O(1)$ Constant** |
| **Insert / Add** | $O(1)$ Amortized | $O(1)$ | **$O(1)$ Constant** |
| **Delete by ID** | $O(N)$ Shift | $O(N)$ Search + $O(1)$ | **$O(1)$ Constant** |
| **Key Uniqueness** | Manual $O(N)$ Check | Manual $O(N)$ Check | **Automatic $O(1)$ Collision Check** |

---

## 📦 Solution Architecture

```
d:\Desktop\Media\School\PROG7312\ICE\ICE 3\
├── ProductInventoryManager.sln          # Visual Studio Solution File
├── ProductInventoryManager.slnx         # Modern .NET XML Solution File
├── .gitignore                           # Git ignore rules for build artifacts
├── README.md                            # Comprehensive documentation & grading guide
│
├── ProductInventoryManager.Core/        # Class Library (.NET 8.0)
│   ├── Models/
│   │   ├── Product.cs                   # Product model (ID, Name, Price, Category, Stock, Validation)
│   │   └── InventoryStatistics.cs       # Aggregated metrics (Total Value, Mean Price, Extremes)
│   ├── Services/
│   │   ├── IInventoryService.cs         # Service interface contract for CRUD operations
│   │   └── DictionaryInventoryService.cs# Thread-safe Dictionary<int, Product> implementation
│   └── Data/
│       └── SampleInventoryData.cs       # 12 seed items including assignment specifications
│
├── ProductInventoryManager.Cli/         # Interactive Console Application (.NET 8.0)
│   └── Program.cs                       # Prompt-based console UI with ANSI styling & menus
│
├── ProductInventoryManager.App/         # Modern Desktop WPF GUI (.NET 8.0-windows)
│   ├── App.xaml / App.xaml.cs           # WPF Application entry point
│   ├── MainWindow.xaml / .cs            # Dashboard, KPI cards, DataGrid, live search & inspector
│   └── Views/
│       ├── ProductDialog.xaml / .cs     # Modal dialog for Add / Edit operations with live validation
│       └── ConfirmDialog.xaml / .cs     # Modal confirmation dialog for product deletion
│
└── ProductInventoryManager.Tests/       # Unit Test Project (xUnit + .NET 8.0)
    ├── ProductTests.cs                  # Validation, formatting, and clone tests (11 tests)
    ├── InventoryServiceTests.cs         # CRUD, uniqueness, and search tests (21 tests)
    └── InventoryStatisticsTests.cs      # Metric calculations and seed tests (6 tests)
```

---

## 📋 Default Seed Inventory (12 Products)

| ProductID | Product Name | Category | Unit Price | Stock | Description |
| :---: | :--- | :--- | :---: | :---: | :--- |
| **101** | **Laptop** *(Required)* | Computers | **R 5,000.00** | 15 | High-performance portable workstation with 16GB RAM. |
| **102** | **JBL Speaker** *(Required)* | Audio | **R 8,000.00** | 25 | Premium wireless Bluetooth speaker with deep bass. |
| **103** | **Tablet** *(Required)* | Mobile | **R 2,300.00** | 30 | 10-inch touchscreen slate ideal for media and browsing. |
| **104** | Wireless Gaming Mouse | Peripherals | R 750.00 | 50 | Ultra-low latency wireless sensor with 16000 DPI. |
| **105** | Mechanical RGB Keyboard | Peripherals | R 1,450.00 | 40 | Tactile switches with customizable per-key lighting. |
| **106** | 27-inch 4K UHD Monitor | Displays | R 6,200.00 | 18 | IPS panel with 99% sRGB color gamut and HDR support. |
| **107** | Noise-Cancelling Headphones | Audio | R 3,500.00 | 22 | Over-ear active noise cancelling with 40-hour battery life. |
| **108** | External 1TB NVMe SSD | Storage | R 1,850.00 | 35 | Rugged USB 3.2 Gen 2 portable drive with 1050MB/s speeds. |
| **109** | Smartwatch Fitness Pro | Wearables | R 4,200.00 | 28 | OLED health tracker with GPS and heart rate monitoring. |
| **110** | USB-C Multiport Dock | Accessories | R 950.00 | 45 | 7-in-1 hub featuring HDMI 4K, PD charging, and USB 3.0. |
| **111** | HD Streaming Webcam | Peripherals | R 1,100.00 | 32 | 1080p 60fps auto-focus camera with stereo microphones. |
| **112** | Ergonomic Office Chair | Furniture | R 3,800.00 | 12 | Breathable mesh back with lumbar support and adjustable arms. |

---

## 🚀 How to Run the Project

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer installed
- Windows 10/11 (for WPF GUI execution)

### 1. Run the Modern WPF GUI
```powershell
dotnet run --project ProductInventoryManager.App
```

### 2. Run the Interactive Console CLI
```powershell
dotnet run --project ProductInventoryManager.Cli
```

### 3. Run the Automated Unit Test Suite
```powershell
dotnet test ProductInventoryManager.sln
```

*Output summary:*
```
Test run for ...\ProductInventoryManager.Tests.dll (.NETCoreApp,Version=v8.0)
Passed!  - Failed: 0, Passed: 38, Skipped: 0, Total: 38, Duration: 43 ms
```

---

## 🖥️ Feature Walkthrough

### 1. Interactive Console Prompts (`ProductInventoryManager.Cli`)
- **[1] Display All Products:** Displays clean formatted table with ProductID, Name, Category, Price in Rand, and Stock.
- **[2] Add New Product:** Step-by-step console prompts with type checking, ID uniqueness validation, and non-empty checks.
- **[3] Search by ProductID:** Instant $O(1)$ key lookup displaying all attributes of the matching product.
- **[4] Search by Keyword:** Case-insensitive substring search matching across names, categories, and descriptions.
- **[5] Update Product Details:** Displays existing values, allows pressing `Enter` to retain existing attributes or entering new values.
- **[6] Delete Product by ID:** Displays product details with a safety `[Y/N]` prompt before removing key from dictionary.
- **[7] Inventory Analytics:** Displays total product count, total stock units, total valuation, mean price, and extreme priced items.
- **[8] Reset Inventory:** Quick reset back to original 12 seed products.
- **[9] Launch WPF GUI:** Opens the modern WPF window directly from the command line.

### 2. Modern WPF GUI Dashboard (`ProductInventoryManager.App`)
- **Top KPI Cards:** Real-time metrics updating immediately when products are added, edited, or deleted:
  - 📦 Total Products in Dictionary
  - 💰 Total Inventory Valuation (ZAR)
  - 📊 Average Unit Price
  - ⭐ Most Expensive Item
- **Live Search & Filters:** Instant reactivity on typing; filter by category dropdown; sort by ID, Name, Price (Low-High / High-Low).
- **Interactive DataGrid:** Dark row hover effects, cyan pill ID tags, emerald green currency tags.
- **Side Inspector Panel:** Live details of the selected item with live Dictionary hash code and technical diagnostics.
- **Action Dialogs:** Add Product modal, Edit Product modal with field-level validation, and Delete confirmation dialog.
- **Toast Feedback:** Bottom notification bar providing immediate visual feedback for all operations.

---

## 🧪 Unit Test Coverage Overview

The test project `ProductInventoryManager.Tests` provides comprehensive coverage:
- **`ProductTests.cs` (11 tests):**
  - Constructor attribute assignments
  - Formatted currency string formatting (`R 5,000.00`)
  - Formatted badge string formatting (`#101`)
  - Positive integer ID validation
  - Non-blank name validation
  - Non-negative price validation
  - Non-negative stock validation
  - Deep clone independence (verifies mutations do not leak)
  - String representation format
- **`InventoryServiceTests.cs` (21 tests):**
  - Seed initialization containing $\ge 10$ products
  - Seed contains required 101 Laptop, 102 JBL Speaker, 103 Tablet
  - Adding valid products increases dictionary count
  - Duplicate key rejection ($O(1)$ `ContainsKey` guard)
  - Null and invalid product rejection
  - `TryGetProduct` returning correct clone on hit and null on miss
  - `GetProductById` behavior
  - `GetAllProducts` ordered dictionary projection
  - Updating existing products (name, price, stock, category)
  - Updating non-existent product handling
  - Updating with invalid parameters rejection
  - Deleting existing product removes key from dictionary
  - Deleting non-existent product handling
  - Search by exact ID and keyword substring matching
  - Filter by category and price bounds
  - Dictionary reset and clear
- **`InventoryStatisticsTests.cs` & `SampleDataTests.cs` (6 tests):**
  - Mathematical accuracy for total inventory value, average price, total units
  - Empty dictionary handling with graceful default zeros
  - Extreme values detection (most expensive & cheapest)
  - Seed dataset uniqueness and validation check

---

## 📄 License & Academic Integrity
This repository is submitted for the **PROG7312 ICE Task 3** assessment. Developed in accordance with institutional academic integrity guidelines.
