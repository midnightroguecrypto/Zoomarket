using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Zoomarket.Controllers;


namespace Zoomarket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Добавляем контроллеры и представления
            builder.Services.AddControllersWithViews();

            // 2. Подключаем базу данных (берем строку из appsettings.json)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 3. Настраиваем Identity (Пользователи и Роли)
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Ослабляем требования к паролю для удобства тестирования
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>() // Говорим Identity использовать нашу БД
            .AddDefaultTokenProviders();

            // 4. Настраиваем куки (чтобы система помнила, кто вошел)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "WarmPaw.Auth";
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = { "Admin", "Manager", "User" };

                foreach (var role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                    }
                }
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // ВАЖНО: Подключаем статические файлы (CSS, картинки)
            app.UseStaticFiles();

            app.UseRouting();

            // ВАЖНО: Аутентификация должна идти ДО Авторизации
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}