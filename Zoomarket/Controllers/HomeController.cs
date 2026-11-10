using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;
using System.Diagnostics;

namespace Zoomarket.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            
            var popularProducts = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Promotions)
                .Take(4)
                .ToListAsync();

            
            var promo = await _context.Promotions.FirstOrDefaultAsync();

            
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.DateCreated)
                .Take(3)
                .ToListAsync();

            
            ViewBag.PopularProducts = popularProducts;
            ViewBag.MainPromo = promo;
            ViewBag.LatestReviews = reviews;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
