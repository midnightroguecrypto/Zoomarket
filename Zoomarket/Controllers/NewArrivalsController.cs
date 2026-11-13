using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    public class NewArrivalsController : Controller
    {
        private readonly AppDbContext _context;

        public NewArrivalsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            var dateLimit = DateTime.UtcNow.AddDays(-14);

            
            var newProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .Where(p => p.DateCreated >= dateLimit)
                .OrderByDescending(p => p.DateCreated) 
                .ToListAsync();

            return View(newProducts);
        }
    }
}