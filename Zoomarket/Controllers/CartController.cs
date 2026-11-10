using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    [Authorize] // Корзина доступна только авторизованным пользователям
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --- ПРОСМОТР КОРЗИНЫ ---
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        // --- ДОБАВЛЕНИЕ В КОРЗИНУ ---
        [HttpPost]
        public async Task<IActionResult> Add(int id, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = id,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // --- УДАЛЕНИЕ ИЗ КОРЗИНЫ ---
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // --- ОБНОВЛЕНИЕ КОЛИЧЕСТВА (Кнопки + / -) ---
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id);
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    _context.CartItems.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // --- ОФОРМЛЕНИЕ ЗАКАЗА ---
        [HttpPost]
        public async Task<IActionResult> Checkout(string address)
        {
            var userId = _userManager.GetUserId(User);

            // 1. Получаем товары из корзины
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index"); // Если корзина пуста, ничего не делаем
            }

            // 2. Создаем заказ
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = "В обработке",
                ShippingAddress = address,
                TotalAmount = cartItems.Sum(c => (c.Product?.Price ?? 0) * c.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. Переносим товары из корзины в OrderItems
            foreach (var item in cartItems)
            {
                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product?.Price ?? 0
                });
            }

            // 4. Очищаем корзину
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // 5. Перенаправляем на страницу успеха
            return RedirectToAction("Success", new { orderId = order.Id });
        }

        // --- СТРАНИЦА УСПЕШНОГО ЗАКАЗА ---
        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}