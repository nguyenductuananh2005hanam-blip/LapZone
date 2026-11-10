using Lapzone_test.Data;
using Lapzone_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lapzone_test.Controllers
{
    public class AccountController : Controller
    {
        public readonly LapDbContext _context;

        public AccountController(LapDbContext context)
        {
            _context = context;
        }

        public string MD5Hash(string password)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                var hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string? email, string? password)
        {
            if(string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng nhập.";
                return View();
            }
            var hashedPassword = MD5Hash(password);
            var account = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);
            if(account != null)
            {
                HttpContext.Session.SetString("UserId", account.Id.ToString());
                HttpContext.Session.SetString("UserName", account.FullName);
                HttpContext.Session.SetString("UserRole", account.Role);
                if(account.Role.Trim() == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng.";
                return View();
            }
        }

        public async Task<IActionResult> Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User? user, string? confirmPassword, bool AgreeToTerms)
        {
            if(string.IsNullOrEmpty(user?.Email) || string.IsNullOrEmpty(user?.Password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin đăng ký.";
                return View();
            }
            if(user?.Password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }
            if(!AgreeToTerms)
            {
                ViewBag.Error = "Vui lòng đồng ý với điều khoản sử dụng.";
                return View();
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ViewBag.Error = "Email đã được sử dụng.";
                return View();
            }

            // Add the new user to the database
            user?.Role = "User";
            user.Password = MD5Hash(user.Password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            ViewBag.Success = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Orders)
                .Include(u => u.CartItems)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpGet]
        public async Task<IActionResult> ChangesPassword(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangesPassword(int? id, string? currentPassword, string? newPassword, string? confirmPassword)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (currentPassword == null || newPassword == null || confirmPassword == null)
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View(user);
            }

            if (MD5Hash(currentPassword) != user.Password)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng.";
                return View(user);
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View(user);
            }

            if(currentPassword == newPassword)
            {
                ViewBag.Error = "Mật khẩu mới không được trùng với mật khẩu hiện tại.";
                return View(user);
            }
            user.Password = MD5Hash(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Đổi mật khẩu thành công!";
            return View(user);
        }
    }
}
