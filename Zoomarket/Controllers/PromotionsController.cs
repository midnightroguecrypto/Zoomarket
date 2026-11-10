using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly AppDbContext _context;

        public PromotionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Promotions
        public async Task<IActionResult> Index()
        {
            // Берем все акции и подгружаем связанные с ними товары
            var promotions = await _context.Promotions
                .Include(p => p.Products)
                .ToListAsync();

            return View(promotions);
        }
    }
}