using Microsoft.AspNetCore.Mvc;
using ECommerceAMY.ViewModels;

namespace ECommerceAMY.ViewComponents
{
    public class AdminDashboardViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(AdminDashboardViewModel model)
        {
            return View(model);
        }
    }
}
