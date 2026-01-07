using ECommerceAMY.Data;
using ECommerceAMY.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _db;
        public CartService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _db.Carts.Add(cart);
                await _db.SaveChangesAsync();
            }
            return cart;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var product = await _db.Products.FindAsync(productId) 
                          ?? throw new InvalidOperationException("Product not found");

            var existing = await _db.CartItems.FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == productId);
            if (existing == null)
            {
                var item = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                _db.CartItems.Add(item);
            }
            else
            {
                existing.Quantity += quantity;
            }
            await _db.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(string userId, int cartItemId, int quantity)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = await _db.CartItems.FirstOrDefaultAsync(i => i.Id == cartItemId && i.CartId == cart.Id)
                       ?? throw new InvalidOperationException("Cart item not found");
            if (quantity <= 0)
            {
                _db.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            await _db.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(string userId, int cartItemId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var item = await _db.CartItems.FirstOrDefaultAsync(i => i.Id == cartItemId && i.CartId == cart.Id);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var items = await _db.CartItems.Where(i => i.CartId == cart.Id).ToListAsync();
            return items.Sum(i => i.UnitPrice * i.Quantity);
        }
    }
}
