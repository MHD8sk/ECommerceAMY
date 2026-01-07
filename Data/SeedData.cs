using ECommerceAMY.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAMY.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;
            var context = provider.GetRequiredService<AppDbContext>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Roles
            string[] roles = new[] { "Admin", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Admin user
            var adminEmail = "admin@shop.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Site Admin"
                };
                await userManager.CreateAsync(adminUser, "Admin#123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Customer users
            var customer1Email = "alice@shop.local";
            if (await userManager.FindByEmailAsync(customer1Email) == null)
            {
                var u = new ApplicationUser { UserName = customer1Email, Email = customer1Email, EmailConfirmed = true, FullName = "Alice" };
                await userManager.CreateAsync(u, "Customer#123");
                await userManager.AddToRoleAsync(u, "Customer");
            }
            var customer2Email = "bob@shop.local";
            if (await userManager.FindByEmailAsync(customer2Email) == null)
            {
                var u = new ApplicationUser { UserName = customer2Email, Email = customer2Email, EmailConfirmed = true, FullName = "Bob" };
                await userManager.CreateAsync(u, "Customer#123");
                await userManager.AddToRoleAsync(u, "Customer");
            }

            // Seed categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new[]
                {
                    new Category { Name = "Electronics" },
                    new Category { Name = "Books" },
                    new Category { Name = "Clothing" },
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed products
            if (!await context.Products.AnyAsync())
            {
                var electronics = await context.Categories.FirstAsync(c => c.Name == "Electronics");
                var books = await context.Categories.FirstAsync(c => c.Name == "Books");
                var clothing = await context.Categories.FirstAsync(c => c.Name == "Clothing");

                // Using local images - add your image files to wwwroot/images/products/
                context.Products.AddRange(
                    new Product { Name = "Wireless Headphones", Description = "Noise-cancelling over-ear headphones.", Price = 199.99m, Stock = 50, CategoryId = electronics.Id, ImageUrl = "/images/products/headphones.jpg" },
                    new Product { Name = "4K Monitor", Description = "27-inch UHD display.", Price = 329.99m, Stock = 30, CategoryId = electronics.Id, ImageUrl = "/images/products/monitor.jpg" },
                    new Product { Name = "C# in Depth", Description = "Advanced C# programming book.", Price = 49.99m, Stock = 100, CategoryId = books.Id, ImageUrl = "/images/products/book.jpg" },
                    new Product { Name = "Men's Jacket", Description = "Warm and stylish.", Price = 89.99m, Stock = 75, CategoryId = clothing.Id, ImageUrl = "/images/products/jacket.jpg" }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
