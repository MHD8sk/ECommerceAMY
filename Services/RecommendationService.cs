using ECommerceAMY.Data;
using ECommerceAMY.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Services
{
    public interface IRecommendationService
    {
        Task<List<Product>> GetRecommendedProductsAsync(string userId, int count = 4);
        Task<List<Product>> GetRelatedProductsAsync(int productId, int count = 4);
        Task<List<Product>> GetTrendingProductsAsync(int count = 8);
        Task<List<Product>> GetFrequentlyBoughtTogetherAsync(int productId, int count = 4);
    }

    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetRecommendedProductsAsync(string userId, int count = 4)
        {
            // Get user's order history to find preferences
            var userCategories = await _context.Orders
                .Where(o => o.UserId == userId)
                .SelectMany(o => o.Items)
                .Select(oi => oi.Product.CategoryId)
                .Distinct()
                .ToListAsync();

            // Get products from user's preferred categories that they haven't ordered
            var orderedProductIds = await _context.Orders
                .Where(o => o.UserId == userId)
                .SelectMany(o => o.Items)
                .Select(oi => oi.ProductId)
                .ToListAsync();

            var recommendations = await _context.Products
                .Where(p => userCategories.Contains(p.CategoryId) && !orderedProductIds.Contains(p.Id))
                .Where(p => p.Stock > 0)
                .OrderByDescending(p => p.Stock)
                .Take(count)
                .ToListAsync();

            return recommendations;
        }

        public async Task<List<Product>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return new List<Product>();

            return await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != productId)
                .Where(p => p.Stock > 0)
                .OrderBy(p => Guid.NewGuid())
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetTrendingProductsAsync(int count = 8)
        {
            // Get products from recent orders (last 30 days)
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            
            var trendingProductIds = await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.CreatedAt >= thirtyDaysAgo)
                .GroupBy(oi => oi.ProductId)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();

            return await _context.Products
                .Where(p => trendingProductIds.Contains(p.Id))
                .Where(p => p.Stock > 0)
                .ToListAsync();
        }

        public async Task<List<Product>> GetFrequentlyBoughtTogetherAsync(int productId, int count = 4)
        {
            // Find products frequently bought together with the given product
            var relatedProductIds = await _context.OrderItems
                .Where(oi1 => oi1.ProductId == productId)
                .Join(_context.OrderItems, 
                    oi1 => oi1.OrderId, 
                    oi2 => oi2.OrderId, 
                    (oi1, oi2) => new { Product1 = oi1.ProductId, Product2 = oi2.ProductId })
                .Where(x => x.Product2 != productId)
                .GroupBy(x => x.Product2)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();

            return await _context.Products
                .Where(p => relatedProductIds.Contains(p.Id))
                .Where(p => p.Stock > 0)
                .ToListAsync();
        }
    }
}
