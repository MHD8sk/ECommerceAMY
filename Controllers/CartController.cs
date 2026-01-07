using ECommerceAMY.Data;
using ECommerceAMY.Models;
using ECommerceAMY.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Controllers
{
    [Authorize(Roles = "Customer,Admin")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(ICartService cartService, AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _db = db;
            _userManager = userManager;
        }

        private string GetUserId() => _userManager.GetUserId(User)!;

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            cart = await _db.Carts.Include(c => c.Items).ThenInclude(i => i.Product).FirstAsync(c => c.Id == cart.Id);
            ViewBag.Total = await _cartService.GetCartTotalAsync(userId);
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return NotFound();
            var userId = GetUserId();
            await _cartService.AddToCartAsync(userId, productId, Math.Max(1, quantity));
            TempData["Toast"] = $"Added '{product.Name}' to cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            await _cartService.UpdateQuantityAsync(GetUserId(), cartItemId, quantity);
            TempData["Toast"] = "Cart updated.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveItemAsync(GetUserId(), cartItemId);
            TempData["Toast"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }
    }
}
