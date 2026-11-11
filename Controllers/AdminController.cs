using Lapzone_test.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Controllers
{
    public class AdminController : Controller
    {
        public readonly LapDbContext _ctx;
        public AdminController(LapDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.CategoryCount = await _ctx.Categories.CountAsync();
            ViewBag.UserCount = await _ctx.Users.CountAsync();
            ViewBag.OrderCount = await _ctx.Orders.CountAsync();
            ViewBag.ProductCount = await _ctx.Products.CountAsync();
            ViewBag.PendingOrders = await _ctx.Orders.Where(o => o.Status == "Chờ xác nhận").CountAsync();
            return View();
        }
    }
}
