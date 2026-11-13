using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return View(reviews);
        }

 
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

  
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

              
                var oneDayAgo = DateTime.UtcNow.AddDays(-1);
                var reviewsToday = await _context.Reviews
                    .Where(r => r.UserId == user.Id && r.DateCreated >= oneDayAgo)
                    .CountAsync();

                if (reviewsToday >= 3)
                {
                    TempData["ReviewStatus"] = "error";
                    TempData["ReviewMessage"] = "Вы уже оставили 3 отзыва за последние 24 часа. Пожалуйста, попробуйте позже.";
                    return RedirectToAction("Index");
                }

                model.UserId = user.Id;
                model.DateCreated = DateTime.UtcNow;

                _context.Reviews.Add(model);
                await _context.SaveChangesAsync();

                TempData["ReviewStatus"] = "success";
                TempData["ReviewMessage"] = "Спасибо! Ваш отзыв успешно опубликован.";
                return RedirectToAction("Index");
            }

            TempData["ReviewStatus"] = "error";
            TempData["ReviewMessage"] = "Пожалуйста, заполните все поля корректно.";
            return View(model);
        }
    }
}