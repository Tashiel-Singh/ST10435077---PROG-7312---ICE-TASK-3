using ProductInventoryManager.Core.Models;

namespace ProductInventoryManager.Core.Data;

/// <summary>
/// Provides default initial seed data satisfying Step 1 of the assignment specification:
/// - Defined Dictionary&lt;int, Product&gt;
/// - Contains at least 10 initial products with IDs 101 (Laptop), 102 (JBL Speaker), 103 (Tablet), etc.
/// </summary>
public static class SampleInventoryData
{
    public static List<Product> GetSeedProducts()
    {
        return new List<Product>
        {
            new Product(101, "Laptop", 5000m, "Computers", 15, "High-performance portable workstation with 16GB RAM."),
            new Product(102, "JBL Speaker", 8000m, "Audio", 25, "Premium wireless Bluetooth speaker with deep bass."),
            new Product(103, "Tablet", 2300m, "Mobile", 30, "10-inch touchscreen slate ideal for media and browsing."),
            new Product(104, "Wireless Gaming Mouse", 750m, "Peripherals", 50, "Ultra-low latency wireless sensor with 16000 DPI."),
            new Product(105, "Mechanical RGB Keyboard", 1450m, "Peripherals", 40, "Tactile switches with customizable per-key lighting."),
            new Product(106, "27-inch 4K UHD Monitor", 6200m, "Displays", 18, "IPS panel with 99% sRGB color gamut and HDR support."),
            new Product(107, "Noise-Cancelling Headphones", 3500m, "Audio", 22, "Over-ear active noise cancelling with 40-hour battery life."),
            new Product(108, "External 1TB NVMe SSD", 1850m, "Storage", 35, "Rugged USB 3.2 Gen 2 portable drive with 1050MB/s speeds."),
            new Product(109, "Smartwatch Fitness Pro", 4200m, "Wearables", 28, "OLED health tracker with GPS and heart rate monitoring."),
            new Product(110, "USB-C Multiport Dock", 950m, "Accessories", 45, "7-in-1 hub featuring HDMI 4K, PD charging, and USB 3.0."),
            new Product(111, "HD Streaming Webcam", 1100m, "Peripherals", 32, "1080p 60fps auto-focus camera with stereo microphones."),
            new Product(112, "Ergonomic Office Chair", 3800m, "Furniture", 12, "Breathable mesh back with lumbar support and adjustable arms.")
        };
    }
}
