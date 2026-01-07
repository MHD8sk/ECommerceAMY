using ECommerceAMY.Models;

namespace ECommerceAMY.ViewModels
{
    public class ProductSearchViewModel
    {
        public string Query { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; } = "Name";
        public bool InStock { get; set; } = false;
        public bool OnSale { get; set; } = false;
        
        public List<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
