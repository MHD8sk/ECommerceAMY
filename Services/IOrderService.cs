using ECommerceAMY.Models;

namespace ECommerceAMY.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderFromCartAsync(string userId);
        Task<List<Order>> GetOrdersForUserAsync(string userId);
        Task<List<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
