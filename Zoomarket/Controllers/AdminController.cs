using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    [Authorize(Roles = "Admin, Manager")] 
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        
        public async Task<IActionResult> Index()
        {
            ViewBag.ProductsCount = await _context.Products.CountAsync();
            ViewBag.OrdersCount = await _context.Orders.CountAsync();
            ViewBag.ReviewsCount = await _context.Reviews.CountAsync();
            ViewBag.UsersCount = await _userManager.Users.CountAsync();
            ViewBag.BrandsCount = await _context.Brands.CountAsync();
            ViewBag.PromotionsCount = await _context.Promotions.CountAsync();
            ViewBag.LeadsCount = await _context.Leads.CountAsync();
            return View();
        }

        // =========================================
        // УПРАВЛЕНИЕ ЗАКАЗАМИ (Доступно Админу и Менеджеру)
        // =========================================
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            
            var users = await _userManager.Users.ToListAsync();
            var userEmails = users.ToDictionary(u => u.Id, u => u.Email);
            ViewBag.UserEmails = userEmails; 

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = $"Статус заказа №{orderId} изменен на '{status}'.";
            }
            return RedirectToAction("Orders");
        }

        // =========================================
        // МОДЕРАЦИЯ ОТЗЫВОВ (Просмотр - Менеджер, Удаление - Админ)
        // =========================================
        public async Task<IActionResult> Reviews()
        {
            var reviews = await _context.Reviews.OrderByDescending(r => r.DateCreated).ToListAsync();
            return View(reviews);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Отзыв удален.";
            }
            return RedirectToAction("Reviews");
        }

        // =========================================
        // УПРАВЛЕНИЕ ПОЛЬЗОВАТЕЛЯМИ (Только Админ)
        // =========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string roleName = roles.FirstOrDefault() ?? "";

               
                string russianRole = roleName switch
                {
                    "Admin" => "Администратор",
                    "Manager" => "Менеджер",
                    "User" => "Пользователь",
                    _ => "Нет роли"
                };

                userRoles[user.Id] = russianRole;
            }
            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!string.IsNullOrEmpty(newRole))
                {
                    await _userManager.AddToRoleAsync(user, newRole);
                }
                TempData["AdminMessage"] = $"Роли для {user.Email} обновлены.";
            }
            return RedirectToAction("Users");
        }

        // =========================================
        // УПРАВЛЕНИЕ ТОВАРАМИ (Только Админ)
        // =========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Products()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Brands = await _context.Brands.ToListAsync();
            ViewBag.Promotions = await _context.Promotions.ToListAsync(); 
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveProduct(Product model, IFormFile? imageFile, int? selectedPromoId)
        {
            if (ModelState.IsValid)
            {
                
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "products");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create)) { await imageFile.CopyToAsync(fileStream); }
                    model.ImageUrl = "/img/products/" + uniqueFileName;
                }

                
                if (model.Id == 0) _context.Products.Add(model);
                else _context.Products.Update(model);
                await _context.SaveChangesAsync(); 

                
                if (selectedPromoId.HasValue && selectedPromoId > 0)
                {
                    var promo = await _context.Promotions.FindAsync(selectedPromoId);
                    if (promo != null)
                    {
                        
                        if (promo.DiscountPercent > 0)
                        {
                            model.OldPrice = model.Price; 
                            model.Price = model.Price * (100 - promo.DiscountPercent) / 100; 
                        }

                        
                        if (!model.Promotions.Any(p => p.Id == promo.Id))
                        {
                            model.Promotions.Add(promo);
                        }

                        _context.Products.Update(model);
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["AdminMessage"] = "Товар сохранен.";
            }
            return RedirectToAction("Products");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Товар удален.";
            }
            return RedirectToAction("Products");
        }

        // =========================================
        // УПРАВЛЕНИЕ БРЕНДАМИ (Только Админ)
        // =========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Brands()
        {
            var brands = await _context.Brands.ToListAsync();
            return View(brands);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveBrand(Brand model, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                if (logoFile != null && logoFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "brands");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + logoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await logoFile.CopyToAsync(fileStream);
                    }
                    model.LogoUrl = "/images/brands/" + uniqueFileName;
                }

                if (model.Id == 0) _context.Brands.Add(model);
                else _context.Brands.Update(model);

                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Бренд сохранен.";
            }
            return RedirectToAction("Brands");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Бренд удален.";
            }
            return RedirectToAction("Brands");
        }

        // =========================================
        // УПРАВЛЕНИЕ АКЦИЯМИ (Только Админ)
        // =========================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Promotions()
        {
            var promotions = await _context.Promotions.ToListAsync();
            return View(promotions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SavePromotion(Promotion model, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
               
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "promotions");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    model.ImageUrl = "/img/promotions/" + uniqueFileName;
                }

               
                if (model.DiscountPercent > 0)
                {
                    model.DiscountText = "-" + model.DiscountPercent + "%";
                }
                else
                {
                    model.DiscountText = ""; 
                }

                if (model.Id == 0) _context.Promotions.Add(model);
                else _context.Promotions.Update(model);

                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Акция сохранена.";
            }
            return RedirectToAction("Promotions");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo != null)
            {
                _context.Promotions.Remove(promo);
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Акция удалена.";
            }
            return RedirectToAction("Promotions");
        }
        public async Task<IActionResult> Leads()
        {
            var leads = await _context.Leads.OrderByDescending(l => l.DateCreated).ToListAsync();
            return View(leads);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLead(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead != null)
            {
                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync();
                TempData["AdminMessage"] = "Заявка удалена.";
            }
            return RedirectToAction("Leads");
        }
    }
}