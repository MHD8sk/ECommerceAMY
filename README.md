# ECommerceAMY

ASP.NET Core MVC e-commerce web application built with .NET 8, Entity Framework Core (SQL Server), and ASP.NET Identity.

## Tech Stack
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core 8 (SQL Server)
- ASP.NET Identity (with Identity UI)
- EF Core Code-First with Migrations
- Bootstrap 5

## Features
- Authentication & Authorization (Identity UI)
  - Roles: Admin, Customer
- Product Catalog: list, details, categories, search
- Shopping Cart: add, update, remove, total
- Orders: checkout, history, status (Pending, Paid, Shipped)
- Admin Panel (Area: Admin)
  - CRUD Products & Categories
  - View/Update Orders
  - Manage Users (assign/remove Admin role)

## Project Structure
- Data: `AppDbContext`, `SeedData`
- Models: `ApplicationUser`, `Product`, `Category`, `Cart`, `CartItem`, `Order`, `OrderItem`
- Services: `ICartService`, `IOrderService` and implementations
- Controllers: `Products`, `Cart`, `Orders`, `Home` + Admin area controllers
- Views: Bootstrap 5 responsive pages

## Local Setup (Windows)
1. Prerequisites
   - .NET SDK 8.x
   - SQL Server or LocalDB (recommended): `(localdb)\\MSSQLLocalDB`

2. Connection String
   - Default is in `appsettings.json` as `DefaultConnection`:
     `Server=(localdb)\\MSSQLLocalDB;Database=ECommerceYahyaDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
   - To use a different SQL Server instance, update the connection string accordingly.

3. Restore and Build
```bash
cd ECommerceYahya
 dotnet restore
 dotnet build
```

4. Create Database (Migrations)
```bash
 dotnet tool install --global dotnet-ef
 dotnet ef database update
```

5. Run
```bash
 dotnet run
```
- Browse: https://localhost:5001 or http://localhost:5000 (see console output)

## Seeded Data
- Roles: `Admin`, `Customer`
- Admin User:
  - Email: `admin@shop.local`
  - Password: `Admin#123`
- Customers:
  - `alice@shop.local` / `Customer#123`
  - `bob@shop.local` / `Customer#123`
- Categories: Electronics, Books, Clothing
- Products: several sample items under categories

## Admin Panel
- Navigate to `/Admin` (requires Admin role). From the dashboard, manage products, categories, orders, and users.

## Notes
- Identity UI is enabled; login/register/logout are available via the navbar.
- Cart is persistent per authenticated user.
- Prices use `decimal(18,2)` in SQL Server.

## Development Tips
- Add new migrations after model changes:
```bash
 dotnet ef migrations add <Name>
 dotnet ef database update
```
- Update `SeedData` if you want more initial products or categories.
