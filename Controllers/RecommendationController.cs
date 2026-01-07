using ECommerceAMY.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAMY.Controllers
{
    public class RecommendationsController : Controller
    {
        private readonly IRecommendationService _recommendationService;

        public RecommendationsController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRelatedProducts(int productId, int count = 4)
        {
            var products = await _recommendationService.GetRelatedProductsAsync(productId, count);
            return PartialView("_ProductGrid", products);
        }

        [HttpGet]
        public async Task<IActionResult> GetTrendingProducts(int count = 8)
        {
            var products = await _recommendationService.GetTrendingProductsAsync(count);
            return PartialView("_ProductGrid", products);
        }

        [HttpGet]
        public async Task<IActionResult> GetFrequentlyBoughtTogether(int productId, int count = 4)
        {
            var products = await _recommendationService.GetFrequentlyBoughtTogetherAsync(productId, count);
            return PartialView("_ProductRecommendations", products);
        }
    }
}
