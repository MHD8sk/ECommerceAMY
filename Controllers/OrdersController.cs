using ECommerceAMY.Models;
using ECommerceAMY.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAMY.Controllers
{
    [Authorize(Roles = "Customer,Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrdersController(IOrderService orderService, ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
        }
        private string GetUserId() => _userManager.GetUserId(User)!;

        public async Task<IActionResult> Checkout()
        {
            ViewBag.Total = await _cartService.GetCartTotalAsync(GetUserId());
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Place()
        {
            var order = await _orderService.CreateOrderFromCartAsync(GetUserId());
            return RedirectToAction("Details", new { id = order.Id });
        }

        public async Task<IActionResult> Details(int id)
        {
            var myOrders = await _orderService.GetOrdersForUserAsync(GetUserId());
            var order = myOrders.FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        public async Task<IActionResult> History()
        {
            var orders = await _orderService.GetOrdersForUserAsync(GetUserId());
            return View(orders);
        }
    }
}
