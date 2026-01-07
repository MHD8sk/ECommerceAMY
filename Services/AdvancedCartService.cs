using ECommerceAMY.Data;
using ECommerceAMY.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Services
{
    public interface IAdvancedCartService : ICartService
    {
        Task<Cart> GetCartWithItemsAsync(string userId);
        Task<CartSummaryViewModel> GetCartSummaryAsync(string userId);
        Task<ShippingEstimate> CalculateShippingAsync(string userId, string postalCode);
        Task ApplyDiscountAsync(string userId, string discountCode);
        Task<CartValidationResult> ValidateCartAsync(string userId);
    }

    public class AdvancedCartService : CartService, IAdvancedCartService
    {
        private readonly AppDbContext _context;

        public AdvancedCartService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartWithItemsAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<CartSummaryViewModel> GetCartSummaryAsync(string userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                return new CartSummaryViewModel();
            }

            var subtotal = cart.Items.Sum(ci => ci.Product.Price * ci.Quantity);
            var shipping = await CalculateShippingAsync(userId, "00000"); // Default postal code
            var tax = subtotal * 0.08m; // 8% tax
            var total = subtotal + shipping.Cost + tax;

            return new CartSummaryViewModel
            {
                Items = cart.Items.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.Price,
                    Quantity = ci.Quantity,
                    Subtotal = ci.Product.Price * ci.Quantity,
                    ImageUrl = ci.Product.ImageUrl,
                    Stock = ci.Product.Stock
                }).ToList(),
                Subtotal = subtotal,
                ShippingCost = shipping.Cost,
                Tax = tax,
                Total = total,
                EstimatedDelivery = shipping.EstimatedDelivery,
                ItemCount = cart.Items.Sum(ci => ci.Quantity)
            };
        }

        public async Task<ShippingEstimate> CalculateShippingAsync(string userId, string postalCode)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart == null || !cart.Items.Any())
            {
                return new ShippingEstimate { Cost = 0, EstimatedDelivery = DateTime.Now.AddDays(1) };
            }

            var totalWeight = cart.Items.Sum(ci => ci.Quantity * 1.5m); // Assume 1.5 lbs per item
            var baseCost = 5.99m;
            var additionalCost = Math.Max(0, (totalWeight - 5) * 0.50m); // $0.50 per lb over 5 lbs

            return new ShippingEstimate
            {
                Cost = baseCost + additionalCost,
                EstimatedDelivery = DateTime.Now.AddDays(totalWeight > 10 ? 3 : 2),
                Method = totalWeight > 10 ? "Express" : "Standard"
            };
        }

        public async Task ApplyDiscountAsync(string userId, string discountCode)
        {
            // Implement discount logic here
            var cart = await GetCartWithItemsAsync(userId);
            if (cart != null)
            {
                // Apply discount based on code
                cart.DiscountCode = discountCode;
                cart.DiscountAmount = discountCode?.ToUpper() == "SAVE10" ? 10.0m : 0;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CartValidationResult> ValidateCartAsync(string userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            var result = new CartValidationResult { IsValid = true };

            if (cart == null || !cart.Items.Any())
            {
                return result;
            }

            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    result.IsValid = false;
                    result.OutOfStockItems.Add(new OutOfStockItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        RequestedQuantity = item.Quantity,
                        AvailableStock = item.Product.Stock
                    });
                }
            }

            return result;
        }
    }

    public class CartSummaryViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int ItemCount { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; }
    }

    public class ShippingEstimate
    {
        public decimal Cost { get; set; }
        public DateTime EstimatedDelivery { get; set; }
        public string Method { get; set; }
    }

    public class CartValidationResult
    {
        public bool IsValid { get; set; }
        public List<OutOfStockItem> OutOfStockItems { get; set; } = new();
    }

    public class OutOfStockItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int RequestedQuantity { get; set; }
        public int AvailableStock { get; set; }
    }
}
