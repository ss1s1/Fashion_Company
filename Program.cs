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

            // ��������� ��������� ������������ � ���������������
            builder.Services.AddControllersWithViews();

            // ���������� �������� Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            // ��������� ������ ��� Entity Framework Core
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

            var app = builder.Build(); //������ ��� ��������� HTTP-��������

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // ������� ��������� ���������� �� ������
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); //����� wwwroot

            app.UseRouting(); //�������������

            // Middleware ��� ������������� Session
            app.UseSession();

            app.UseAuthentication(); // ��������� ��������������
            app.UseAuthorization(); // ��������� �����������

            // ����������� �������� �� ���������
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // ����������� �������� ��� ��������� �� ������
            app.MapControllerRoute(
                name: "catalogByAuthor",
                pattern: "Catalog/ByAuthor",
                defaults: new { controller = "Catalog", action = "CatalogByAuthor" });



            app.Run();
        }
    }
}
