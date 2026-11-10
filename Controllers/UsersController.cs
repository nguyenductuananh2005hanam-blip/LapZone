
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lapzone_test.Models;
using Lapzone_test.Data;

public class UsersController : Controller
{
    private readonly LapDbContext _context;

    public UsersController(LapDbContext context)
    {
        _context = context;
    }

    // GET: USERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Users.ToListAsync());
    }

    // GET: USERS/Details/5
    public async Task<IActionResult> Details(int? id)
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

    // GET: USERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: USERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,FullName,Email,Phone,Password,Address,Role,CartItems,Orders")] User user)
    {
        if (ModelState.IsValid)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: USERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
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
        return View(user);
    }

    // POST: USERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,FullName,Email,Phone,Password,Address,Role,CartItems,Orders")] User model)
    {
        if (id != model.Id) return NotFound();

        ModelState.Remove("Password");
        ModelState.Remove("Role");
        ModelState.Remove("CartItems");
        ModelState.Remove("Orders");
        ModelState.Remove("Email");

        if (!ModelState.IsValid) return View(model);

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Chỉ cập nhật những trường được phép sửa
        user.FullName = model.FullName;
        user.Phone = model.Phone;
        user.Address = model.Address;

        await _context.SaveChangesAsync();

        if (HttpContext.Session.GetString("UserId") == user.Id.ToString())
            HttpContext.Session.SetString("UserName", user.FullName);

        return RedirectToAction("Profile", "Account", new { id = user.Id });
    }

    // GET: USERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: USERS/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int? id)
    //{
    //    var user = await _context.Users.FindAsync(id);
    //    if (user != null)
    //    {
    //        _context.Users.Remove(user);
    //    }

    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int id, string role)
    {
        var allowedRoles = new[] { "Admin", "User" }; // phải khớp với danh sách ở view
        if (!allowedRoles.Contains(role))
        {
            return BadRequest("Role không hợp lệ.");
        }

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Role = role;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã cập nhật quyền của {user.FullName} thành {role}.";
        return RedirectToAction(nameof(Index));
    }
    private bool UserExists(int? id)
    {
        return _context.Users.Any(e => e.Id == id);
    }
}
