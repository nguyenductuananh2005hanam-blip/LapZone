
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lapzone_test.Models;
using Lapzone_test.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class ProductsController : Controller
{
    private readonly LapDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ProductsController(LapDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: PRODUCTS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.ProductDetail).ToListAsync());
    }

    // GET: PRODUCTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.ProductDetail)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    // GET: PRODUCTS/Create
    public IActionResult Create()
    {
        ViewBag.Catogies = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

    // POST: PRODUCTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Image,CategoryId,CartItems,Category,OrderDetails,ProductDetail")] Product product, IFormFile? HinhAnh)
    {
        ModelState.Remove("Image");
        ModelState.Remove("Category");
        ModelState.Remove("CartItems");
        ModelState.Remove("OrderDetails");
        ModelState.Remove("ProductDetail.Product");


        if (ModelState.IsValid)
        {
            var folder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(HinhAnh!.FileName).ToLowerInvariant()}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await HinhAnh.CopyToAsync(stream);
            }
            product.Image = fileName;

            try
            {
                // EF tự lưu product trước, rồi gán product_id cho product_details
                _context.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Lỗi DB thì xóa ảnh vừa upload
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                ModelState.AddModelError(string.Empty, "Không thể lưu sản phẩm. Vui lòng thử lại.");
                ViewBag.Catogies = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            return RedirectToAction(nameof(Index));
        }
        ViewBag.Catogies = new SelectList(_context.Categories, "Id", "Name");
        return View(product);
    }

    // GET: PRODUCTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }
        ViewBag.Catogies = new SelectList(_context.Categories, "Id", "Name");
        var product = await _context.Products.AsNoTracking().Include(x => x.ProductDetail).FirstOrDefaultAsync(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    // POST: PRODUCTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,Name,Price,Stock,CategoryId,ProductDetail")] Product product,
        IFormFile? HinhAnh)
    {
        if (id != product.Id) return NotFound();

        // Bỏ qua validate các field form không gửi lên
        ModelState.Remove("Image");
        ModelState.Remove("Category");
        ModelState.Remove("CartItems");
        ModelState.Remove("OrderDetails");
        ModelState.Remove("ProductDetail.Product");


        // Lấy bản ghi hiện có trong DB
        var existing = await _context.Products
            .Include(p => p.ProductDetail)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null) return NotFound();

        if (!ModelState.IsValid)
        {
            product.Image = existing.Image; // giữ ảnh cũ để hiển thị lại
            ViewBag.Catogies = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ===== Cập nhật bảng products =====
        existing.Name = product.Name;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        existing.CategoryId = product.CategoryId;

        // ===== Cập nhật bảng product_details =====
        if (product.ProductDetail != null)
        {
            // Sản phẩm cũ chưa có chi tiết thì tạo mới
            existing.ProductDetail ??= new ProductDetail();

            var d = existing.ProductDetail;
            var s = product.ProductDetail;

            d.Cpu = s.Cpu;
            d.Ram = s.Ram;
            d.Storage = s.Storage;
            d.Gpu = s.Gpu;
            d.Screen = s.Screen;
            d.Battery = s.Battery;
            d.Audio = s.Audio;
            d.Ports = s.Ports;
            d.Wireless = s.Wireless;
            d.Weight = s.Weight;
            d.Color = s.Color;
            d.Os = s.Os;
            d.ConditionStatus = s.ConditionStatus;
            d.Warranty = s.Warranty;
            d.MoreInfo = s.MoreInfo;
        }

        // ===== Xử lý ảnh =====
        string? oldImage = null;
        string? newFilePath = null;

        if (HinhAnh != null && HinhAnh.Length > 0)
        {
            var folder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(HinhAnh.FileName).ToLowerInvariant()}";
            newFilePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(newFilePath, FileMode.Create))
            {
                await HinhAnh.CopyToAsync(stream);
            }

            oldImage = existing.Image;
            existing.Image = fileName;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (newFilePath != null && System.IO.File.Exists(newFilePath))
                System.IO.File.Delete(newFilePath);

            if (!await _context.Products.AnyAsync(p => p.Id == id))
                return NotFound();
            throw;
        }
        catch (DbUpdateException)
        {
            if (newFilePath != null && System.IO.File.Exists(newFilePath))
                System.IO.File.Delete(newFilePath);

            ModelState.AddModelError(string.Empty, "Không thể lưu thay đổi. Vui lòng thử lại.");
            product.Image = oldImage ?? existing.Image;
            ViewBag.Catogies = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // Lưu thành công thì xóa ảnh cũ (nếu đã thay ảnh mới)
        if (!string.IsNullOrEmpty(oldImage))
        {
            var oldPath = Path.Combine(_env.WebRootPath, "images", Path.GetFileName(oldImage));
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: PRODUCTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            ViewBag.ErrorMessage = "Không tìm thấy sản phẩm để xóa.";
            return NotFound();
        }

        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
        else
        {
            ViewBag.ErrorMessage = "Không tìm thấy sản phẩm để xóa.";
            return NotFound();
        }
        ViewBag.Success = "Xóa sản phẩm thành công.";
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: PRODUCTS/Delete/5
    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteConfirmed(int? id)
    //{
    //    var product = await _context.Products.FindAsync(id);
    //    if (product != null)
    //    {
    //        _context.Products.Remove(product);
    //    }

    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}

    private bool ProductExists(int? id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
