using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    public class BrandsController : Controller
    {
        private readonly AppDbContext _context;

        public BrandsController(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var brands = await _context.Brands.ToListAsync();
            return View(brands);
        }

        
        public async Task<IActionResult> Products(string brandName)
        {
            if (string.IsNullOrEmpty(brandName))
            {
                return RedirectToAction("Index");
            }

            
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .Where(p => p.Brand == brandName)
                .ToListAsync();

           
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Name == brandName);

            ViewBag.BrandName = brandName;
            ViewBag.BrandLogo = brand?.LogoUrl ?? "https://cdn-icons-png.flaticon.com/512/616/616516.png"; 

            
            ViewBag.AllBrands = await _context.Brands.ToListAsync();

            return View(products);
        }

    }
}