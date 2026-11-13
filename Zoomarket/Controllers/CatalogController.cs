using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(int? categoryId, string? search, decimal? minPrice, decimal? maxPrice)
        {
           
            var query = _context.Products.Include(p => p.Category).Include(p => p.Promotions).AsQueryable();

            
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;

            var products = await query.ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            
            var relatedProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(8) 
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }
    }
}