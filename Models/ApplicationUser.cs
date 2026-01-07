using Microsoft.AspNetCore.Identity;

namespace ECommerceAMY.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extend with profile fields if needed
        public string? FullName { get; set; }
    }
}
