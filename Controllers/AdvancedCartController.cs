using ECommerceAMY.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAMY.Controllers
{
    [Authorize]
    public class AdvancedCartController : Controller
    {
        private readonly IAdvancedCartService _cartService;

        public AdvancedCartController(IAdvancedCartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.Name ?? "";
            var cartSummary = await _cartService.GetCartSummaryAsync(userId);
            return View(cartSummary);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            var userId = User.Identity?.Name ?? "";
            await _cartService.UpdateQuantityAsync(userId, productId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyDiscount([FromBody] DiscountRequest request)
        {
            var userId = User.Identity?.Name ?? "";
            await _cartService.ApplyDiscountAsync(userId, request.DiscountCode);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCart()
        {
            var userId = User.Identity?.Name ?? "";
            var result = await _cartService.ValidateCartAsync(userId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetShippingEstimate(string postalCode)
        {
            var userId = User.Identity?.Name ?? "";
            var estimate = await _cartService.CalculateShippingAsync(userId, postalCode);
            return Json(estimate);
        }
    }

    public class DiscountRequest
    {
        public string DiscountCode { get; set; }
    }
}
