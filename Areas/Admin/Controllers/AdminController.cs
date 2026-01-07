using ECommerceAMY.Data;
using ECommerceAMY.Models;
using ECommerceAMY.Services;
using ECommerceAMY.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecommendationService _recommendationService;
        
        public AdminController(AppDbContext db, UserManager<ApplicationUser> userManager, IRecommendationService recommendationService)
        {
            _db = db;
            _userManager = userManager;
            _recommendationService = recommendationService;
        }

        public async Task<IActionResult> Index()
        {
            // Basic stats
            var totalOrders = await _db.Orders.CountAsync();
            var totalRevenue = await _db.Orders.SumAsync(o => o.Total);
            var totalProducts = await _db.Products.CountAsync();
            var totalUsers = await _userManager.Users.CountAsync();
            var pendingOrders = await _db.Orders.Where(o => o.Status == OrderStatus.Pending).CountAsync();
            
            // Recent activity
            var recentOrders = await _db.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .ToListAsync();
                
            var lowStockProducts = await _db.Products
                .Where(p => p.Stock < 10 && p.Stock > 0)
                .OrderBy(p => p.Stock)
                .Take(10)
                .ToListAsync();
                
            var newCustomers = await _userManager.Users
                .OrderByDescending(u => u.Email) // Use Email instead of CreatedAt
                .Take(5)
                .ToListAsync();
            
            // Sales data for charts - SQL-friendly query first
            var rawMonthlySales = await _db.Orders
                .Where(o => o.CreatedAt >= DateTime.Now.AddMonths(-12))
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(o => o.Total),
                    Orders = g.Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .ToListAsync();
            
            // Then format in C# after SQL query
            var monthlySales = rawMonthlySales.Select(x => new SalesData
            {
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                Revenue = x.Revenue,
                Orders = x.Orders
            }).ToList();
            
            // Top products
            var topProducts = await _db.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProduct
                {
                    ProductId = g.Key,
                    ProductName = _db.Products.First(p => p.Id == g.Key).Name,
                    ImageUrl = _db.Products.First(p => p.Id == g.Key).ImageUrl,
                    UnitsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
                    GrowthRate = 15.5 // Placeholder growth rate
                })
                .OrderByDescending(tp => tp.Revenue)
                .Take(8)
                .ToListAsync();
            
            // Top categories
            var topCategories = await _db.OrderItems
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .GroupBy(oi => oi.Product.CategoryId)
                .Select(g => new TopCategory
                {
                    CategoryName = _db.Categories.First(c => c.Id == g.Key).Name,
                    ProductCount = _db.Products.Count(p => p.CategoryId == g.Key),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice),
                    Percentage = (double)g.Sum(oi => oi.Quantity * oi.UnitPrice) * 100 / (double)totalRevenue
                })
                .OrderByDescending(tc => tc.Revenue)
                .Take(5)
                .ToListAsync();
            
            // Performance metrics
            var averageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0;
            var conversionRate = 3.2; // Placeholder conversion rate
            var cartAbandonmentRate = 28; // Placeholder cart abandon rate
            var customerRetentionRate = 75; // Placeholder retention rate

            var dashboard = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts,
                TotalCustomers = totalUsers,
                PendingOrders = pendingOrders,
                RecentOrders = recentOrders,
                LowStockProducts = lowStockProducts,
                NewCustomers = newCustomers,
                MonthlySales = monthlySales,
                TopProducts = topProducts,
                TopCategories = topCategories,
                AverageOrderValue = averageOrderValue,
                ConversionRate = conversionRate,
                CartAbandonmentRate = cartAbandonmentRate,
                CustomerRetentionRate = customerRetentionRate
            };

            return View(dashboard);
        }
    }
}
