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
            return View();
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
    }
}
