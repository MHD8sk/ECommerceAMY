using ECommerceAMY.Models;

namespace ECommerceAMY.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Overview Stats
        public int TotalOrders { get; set; } = default!;
        public decimal TotalRevenue { get; set; } = default!;
        public int TotalCustomers { get; set; } = default!;
        public int TotalProducts { get; set; } = default!;
        public int PendingOrders { get; set; } = default!;
        
        // Recent Activity
        public List<Order> RecentOrders { get; set; } = new();
        public List<Product> LowStockProducts { get; set; } = new();
        public List<ApplicationUser> NewCustomers { get; set; } = new();
        
        // Sales Data
        public List<SalesData> MonthlySales { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<TopCategory> TopCategories { get; set; } = new();
        
        // Performance Metrics
        public decimal AverageOrderValue { get; set; } = default!;
        public double ConversionRate { get; set; } = default!;
        public int CartAbandonmentRate { get; set; } = default!;
        public int CustomerRetentionRate { get; set; } = default!;
    }

    public class SalesData
    {
        public string Month { get; set; } = default!;
        public decimal Revenue { get; set; } = default!;
        public int Orders { get; set; } = default!;
    }

    public class TopProduct
    {
        public int ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public int UnitsSold { get; set; } = default!;
        public decimal Revenue { get; set; } = default!;
        public double GrowthRate { get; set; } = default!;
    }

    public class TopCategory
    {
        public string CategoryName { get; set; } = default!;
        public int ProductCount { get; set; } = default!;
        public decimal Revenue { get; set; } = default!;
        public double Percentage { get; set; } = default!;
    }
}
