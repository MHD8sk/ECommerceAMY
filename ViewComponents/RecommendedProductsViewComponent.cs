using ECommerceAMY.Models;
using ECommerceAMY.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAMY.ViewComponents
{
    public class RecommendedProductsViewComponent : ViewComponent
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendedProductsViewComponent(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, int count = 4)
        {
            var products = await _recommendationService.GetRecommendedProductsAsync(userId, count);
            return View(products);
        }
    }
}
