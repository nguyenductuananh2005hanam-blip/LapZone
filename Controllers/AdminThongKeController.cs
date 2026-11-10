using Lapzone_test.Data;
using Lapzone_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Controllers
{
    public class AdminThongKeController : Controller
    {
        private readonly LapDbContext _context;

        public AdminThongKeController(LapDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}
