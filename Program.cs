using Fashion_Company.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Fashion_Company.Models;
using Microsoft.AspNetCore.Identity;



namespace Fashion_Company
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Добавляем поддержку контроллеров с представлениями
            builder.Services.AddControllersWithViews();

            // Добавление сервисов Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // Добавляем службу для Entity Framework Core
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



            builder.Services.AddAuthentication("Cookies")
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.AccessDeniedPath = "/User/AccessDenied";
                });



            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            var app = builder.Build(); //сервер для обработки HTTP-запросов

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Покажет детальную информацию об ошибке
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //папка wwwroot

            app.UseRouting(); //маршрутизация

            // Middleware для использования Session
            app.UseSession();

            app.UseAuthentication(); // Добавляем аутентификацию
            app.UseAuthorization(); // Добавляем авторизацию

            // Регистрация маршрута по умолчанию
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Регистрация маршрута для каталогов по автору
            app.MapControllerRoute(
                name: "catalogByAuthor",
                pattern: "Catalog/ByAuthor",
                defaults: new { controller = "Catalog", action = "CatalogByAuthor" });



            app.Run();
        }
    }
}
