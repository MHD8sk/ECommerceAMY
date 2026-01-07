using ECommerceAMY.Models;

namespace ECommerceAMY.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(string userId);
        Task AddToCartAsync(string userId, int productId, int quantity);
        Task UpdateQuantityAsync(string userId, int cartItemId, int quantity);
        Task RemoveItemAsync(string userId, int cartItemId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}
