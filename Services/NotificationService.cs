using ECommerceAMY.Data;
using ECommerceAMY.Models;
using ECommerceAMY.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Services
{
    public interface INotificationService
    {
        Task NotifyOrderPlaced(Order order);
        Task NotifyOrderStatusChanged(Order order);
        Task NotifyLowStock(Product product);
    }

    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly AppDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, AppDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task NotifyOrderPlaced(Order order)
        {
            await _hubContext.Clients.User(order.UserId).SendAsync("OrderPlaced", new
            {
                OrderId = order.Id,
                Message = $"Your order #{order.Id} has been placed successfully!",
                Total = order.Total.ToString("C")
            });
        }

        public async Task NotifyOrderStatusChanged(Order order)
        {
            await _hubContext.Clients.User(order.UserId).SendAsync("OrderStatusChanged", new
            {
                OrderId = order.Id,
                Status = order.Status,
                Message = $"Order #{order.Id} status updated to: {order.Status}"
            });
        }

        public async Task NotifyLowStock(Product product)
        {
            var admins = await _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Admin").Id))
                .ToListAsync();

            foreach (var admin in admins)
            {
                await _hubContext.Clients.User(admin.Id).SendAsync("LowStockAlert", new
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Stock = product.Stock,
                    Message = $"Low stock alert: {product.Name} has only {product.Stock} items left!"
                });
            }
        }
    }
}
