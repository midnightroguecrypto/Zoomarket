using Microsoft.AspNetCore.Mvc;
using Zoomarket.Models;

namespace Zoomarket.Controllers
{
    public class LeadController : Controller
    {
        private readonly AppDbContext _context;

        public LeadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lead model)
        {
            if (ModelState.IsValid)
            {
                _context.Leads.Add(model);
                await _context.SaveChangesAsync();

                TempData["LeadSuccess"] = "Спасибо! Мы свяжемся с вами в ближайшее время.";
                return RedirectToAction("Thanks"); 
            }

            
            TempData["LeadError"] = "Пожалуйста, заполните все поля корректно.";
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult Thanks()
        {
            return View();
        }
    }
}