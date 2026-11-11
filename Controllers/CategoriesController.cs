
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lapzone_test.Models;
using Lapzone_test.Data;

public class CategoriesController : Controller
{
    private readonly LapDbContext _context;

    public CategoriesController(LapDbContext context)
    {
        _context = context;
    }

    // GET: CATEGORYS
    public async Task<IActionResult> Index()    
    {
        var adminrole = HttpContext.Session.GetString("UserRole");
        if (adminrole != "Admin")
        {
            return RedirectToAction("Login", "Account");
        }
        return View(await _context.Categories.OrderBy(c => c.Id).ToListAsync());
    }

    // GET: CATEGORYS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    // GET: CATEGORYS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CATEGORYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Products")] Category category)
    {
        ModelState.Remove("Products");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                ViewBag.Success = "Thêm danh mục thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra khi thêm danh mục.";
                return View(category);
            }
        }
        return View(category);
    }

    // GET: CATEGORYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    // POST: CATEGORYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Products")] Category category)
    {
        ModelState.Remove("Products");
        if (id != category.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: CATEGORYS/Delete
    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            if (id == null) return NotFound();
            var danhMuc = await _context.Categories.FindAsync(id);
            if (danhMuc == null) return NotFound();
            ViewBag.Success = "Xóa danh mục thành công.";
            _context.Categories.Remove(danhMuc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Có lỗi xảy ra khi xóa danh mục: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
    // POST: CATEGORYS/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int? id)
    //{
    //    var category = await _context.Categories.FindAsync(id);
    //    if (category != null)
    //    {
    //        _context.Categories.Remove(category);
    //    }

    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

    private bool CategoryExists(int? id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
