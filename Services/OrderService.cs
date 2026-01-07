using ECommerceAMY.Data;
using ECommerceAMY.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;
        private readonly ICartService _cartService;
        public OrderService(AppDbContext db, ICartService cartService)
        {
            _db = db;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId)
        {
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            await _db.Entry(cart).Collection(c => c.Items).LoadAsync();
            foreach (var item in cart.Items)
            {
                await _db.Entry(item).Reference(i => i.Product).LoadAsync();
            }

            if (!cart.Items.Any())
            {
                throw new InvalidOperationException("Cart is empty");
            }

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Total = cart.Items.Sum(i => i.UnitPrice * i.Quantity)
            };

            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });

                if (item.Product != null)
                {
                    item.Product.Stock = Math.Max(0, item.Product.Stock - item.Quantity);
                }
            }

            _db.Orders.Add(order);

            _db.CartItems.RemoveRange(cart.Items);
            await _db.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetOrdersForUserAsync(string userId)
        {
            return await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _db.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Include(o => o.User)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(orderId) ?? throw new InvalidOperationException("Order not found");
            order.Status = status;
            await _db.SaveChangesAsync();
        }
    }
}
