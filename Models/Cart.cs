namespace ECommerceAMY.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public string? DiscountCode { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
