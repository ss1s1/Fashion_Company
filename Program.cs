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

            // Äîáàâëÿåì ïîääåðæêó êîíòðîëëåðîâ ñ ïðåäñòàâëåíèÿìè
            builder.Services.AddControllersWithViews();

            // Äîáàâëåíèå ñåðâèñîâ Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                ?? builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));


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

            var app = builder.Build(); //ñåðâåð äëÿ îáðàáîòêè HTTP-çàïðîñîâ

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Ïîêàæåò äåòàëüíóþ èíôîðìàöèþ îá îøèáêå
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //ïàïêà wwwroot

            app.UseRouting(); //ìàðøðóòèçàöèÿ

            // Middleware äëÿ èñïîëüçîâàíèÿ Session
            app.UseSession();

            app.UseAuthentication(); // Äîáàâëÿåì àóòåíòèôèêàöèþ
            app.UseAuthorization(); // Äîáàâëÿåì àâòîðèçàöèþ

            // Ðåãèñòðàöèÿ ìàðøðóòà ïî óìîë÷àíèþ
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Ðåãèñòðàöèÿ ìàðøðóòà äëÿ êàòàëîãîâ ïî àâòîðó
            app.MapControllerRoute(
                name: "catalogByAuthor",
                pattern: "Catalog/ByAuthor",
                defaults: new { controller = "Catalog", action = "CatalogByAuthor" });



            app.Run();
        }
    }
}
