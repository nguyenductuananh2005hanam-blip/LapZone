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
    private async Task<List<CartItem>> LoadCart(int userId)
    {
        return await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Account");

        var cartItems = await LoadCart(userId.Value);
        if (!cartItems.Any()) return RedirectToAction(nameof(Index));

        var user = await _context.Users.FindAsync(userId.Value);

        ViewBag.CartItems = cartItems;
        // điền sẵn thông tin từ tài khoản
        return View(new Order { CustomerName = user?.FullName, CustomerPhone = user?.Phone, Address = user?.Address });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(string? name,string? phone, string? address, Order order)
    {
        ModelState.Remove(nameof(order.User));
        ModelState.Remove(nameof(order.OrderDate));
        if(string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address))
        {
            TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
        }
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToAction("Login", "Account");
        var cartItems = await _context.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
        if (!cartItems.Any())
        {
            TempData["Error"] = "Giỏ hàng trống, vui lòng thêm sản phẩm trước khi thanh toán.";
            return View(order);
        }
        order.CustomerName = name;
        order.CustomerPhone = phone;
        order.Address = address;
        order.Status = "Đang xử lý";

        order.UserId = userId;
        order.OrderDate = DateTime.Now;
        order.TotalAmount = cartItems.Sum(c => c.Product.Price * c.Quantity);
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        foreach (var item in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product.Price
            };
            _context.OrderDetails.Add(orderDetail);
        }
        cartItems.ForEach(c => c.Product.Stock -= c.Quantity); // giảm số lượng tồn kho
        _context.CartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đặt hàng thành công!";
        return RedirectToAction("Index", "Home");
    }
    [HttpGet]
    public async Task<IActionResult> OderDetails(int? id)
    {
        var userId = GetCurrentUserId();
        if(userId == null) return RedirectToAction("Login", "Account");

        var orders = await _context.Orders
        .Where(o => o.UserId == userId)
        .OrderByDescending(o => o.OrderDate)   
        .ToListAsync();
        return View(orders);
    }

}
