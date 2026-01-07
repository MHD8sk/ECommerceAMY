using ECommerceAMY.Models;
using ECommerceAMY.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAMY.ViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartSummaryViewComponent(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return View((0, 0m));
            }
            var userId = _userManager.GetUserId(HttpContext.User)!;
            var cart = await _cartService.GetOrCreateCartAsync(userId);
            var itemCount = cart.Items.Sum(i => i.Quantity);
            var total = await _cartService.GetCartTotalAsync(userId);
            return View((itemCount, total));
        }
    }
}
