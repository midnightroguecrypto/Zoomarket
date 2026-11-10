using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public ProfileController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // --- ВКЛАДКА "МОИ ЗАКАЗЫ" ---
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Очистка старых отмененных заказов
            var cutoffDate = DateTime.UtcNow.AddDays(-30);
            var oldCancelledOrders = await _context.Orders
                .Where(o => o.UserId == user.Id && o.Status == "Отменен" && o.OrderDate < cutoffDate)
                .ToListAsync();
            if (oldCancelledOrders.Any())
            {
                _context.Orders.RemoveRange(oldCancelledOrders);
                await _context.SaveChangesAsync();
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.FullName = profile?.FullName ?? user.Email;
            ViewBag.ActiveTab = "Orders";

            return View("Index", orders);
        }

        // --- ВКЛАДКА "МОИ ОТЗЫВЫ" ---
        public async Task<IActionResult> Reviews()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var reviews = await _context.Reviews
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            ViewBag.UserName = user.UserName;
            ViewBag.Email = user.Email;
            ViewBag.FullName = profile?.FullName ?? user.Email;
            ViewBag.ActiveTab = "Reviews";

            return View("Reviews", reviews);
        }

        // --- ОТМЕНА ЗАКАЗА ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId, string reason, string? otherReason)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

            if (order != null && order.Status != "Доставлен" && order.Status != "Отменен")
            {
                order.Status = "Отменен";
                order.CancelReason = (reason == "Другое" && !string.IsNullOrWhiteSpace(otherReason)) ? otherReason : reason;
                await _context.SaveChangesAsync();
                TempData["ProfileMessage"] = "Заказ успешно отменен.";
                TempData["ProfileStatus"] = "success";
            }
            else
            {
                TempData["ProfileMessage"] = "Не удалось отменить заказ.";
                TempData["ProfileStatus"] = "error";
            }

            return RedirectToAction("Index");
        }

        // --- УДАЛЕНИЕ ОТЗЫВА ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == user.Id);

            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["ProfileMessage"] = "Отзыв успешно удален.";
                TempData["ProfileStatus"] = "success";
            }
            else
            {
                TempData["ProfileMessage"] = "Не удалось удалить отзыв.";
                TempData["ProfileStatus"] = "error";
            }

            return RedirectToAction("Reviews");
        }
    }
}