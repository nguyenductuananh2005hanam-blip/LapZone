
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lapzone_test.Models;
using Lapzone_test.Data;

public class ProductDetailsController : Controller
{
    private readonly LapDbContext _context;

    public ProductDetailsController(LapDbContext context)
    {
        _context = context;
    }

    // GET: PRODUCTDETAILS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ProductDetails.ToListAsync());
    }

    // GET: PRODUCTDETAILS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productdetail = await _context.ProductDetails
            .FirstOrDefaultAsync(m => m.Id == id);
        if (productdetail == null)
        {
            return NotFound();
        }

        return View(productdetail);
    }

    // GET: PRODUCTDETAILS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PRODUCTDETAILS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Cpu,Ram,Storage,Gpu,Screen,Battery,Audio,Ports,Wireless,Weight,Color,Os,ConditionStatus,Warranty,MoreInfo,ProductId,Product")] ProductDetail productdetail)
    {
        if (ModelState.IsValid)
        {
            _context.Add(productdetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(productdetail);
    }

    // GET: PRODUCTDETAILS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productdetail = await _context.ProductDetails.FindAsync(id);
        if (productdetail == null)
        {
            return NotFound();
        }
        return View(productdetail);
    }

    // POST: PRODUCTDETAILS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Cpu,Ram,Storage,Gpu,Screen,Battery,Audio,Ports,Wireless,Weight,Color,Os,ConditionStatus,Warranty,MoreInfo,ProductId,Product")] ProductDetail productdetail)
    {
        if (id != productdetail.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(productdetail);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductDetailExists(productdetail.Id))
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
        return View(productdetail);
    }

    // GET: PRODUCTDETAILS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productdetail = await _context.ProductDetails
            .FirstOrDefaultAsync(m => m.Id == id);
        if (productdetail == null)
        {
            return NotFound();
        }

        return View(productdetail);
    }

    // POST: PRODUCTDETAILS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var productdetail = await _context.ProductDetails.FindAsync(id);
        if (productdetail != null)
        {
            _context.ProductDetails.Remove(productdetail);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProductDetailExists(int? id)
    {
        return _context.ProductDetails.Any(e => e.Id == id);
    }
}
