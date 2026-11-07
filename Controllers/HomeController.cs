using Lapzone_test.Data;
using Lapzone_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Lapzone_test.Controllers
{
    public class HomeController : Controller
    {
        public readonly LapDbContext _ctx;

        public HomeController(LapDbContext ctx)
        {
            _ctx = ctx;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _ctx.Products.OrderByDescending(x => x.Id).Take(12).ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> About()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> DsDanhMuc()
        {
            var categories = await _ctx.Categories.ToListAsync();
            return View(categories);
        }


        public async Task<IActionResult> DsSanPham()
        {
            var products = await _ctx.Products.OrderByDescending(x => x.Id).Take(12).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductsByCategories(int categoryId)
        {
            var products = await _ctx.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> ProductDetails(int? productId)
        {
            var product = await _ctx.Products
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
