
using Lapzone_test.Data;
using Lapzone_test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.NativeInterop;
using System.Security.Claims;

public class CartItemsController : Controller
{
    private readonly LapDbContext _context;

    public CartItemsController(LapDbContext context)
    {
        _context = context;
    }
    private int? GetCurrentUserId()
    {
        var idStr = HttpContext.Session.GetString("UserId");
        return int.TryParse(idStr, out var id) ? id : null;
    }
    // GET: CARTITEMS
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Account");

        var items = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? returnUrl = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Account");

        if (quantity < 1) quantity = 1;

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound();

        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

        if (item != null)
        {
            item.Quantity += quantity;          // đã có thì cộng dồn
        }
        else
        {
            _context.CartItems.Add(new CartItem
            {
                UserId = userId.Value,
                ProductId = productId,
                Quantity = quantity
            });
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // POST: cập nhật số lượng
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int id, int quantity)
    {
        var userId = GetCurrentUserId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (item == null) return NotFound();

        if (quantity < 1)
            _context.CartItems.Remove(item);
        else
            item.Quantity = quantity;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: xóa khỏi giỏ
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = GetCurrentUserId();
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
