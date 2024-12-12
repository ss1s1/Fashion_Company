using Fashion_Company.Data;
using Fashion_Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fashion_Company.Controllers
{
    [Authorize(Roles = "Admin")] // Только для админов
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Страница для администратора
        public async Task<IActionResult> Index()
        {
            // Получаем всех пользователей без пароля
            var users = await _context.Users.ToListAsync();
            var userWithCatalogs = new List<UserWithCatalogsViewModel>();

            foreach (var user in users)
            {
                // Получаем каталоги для каждого пользователя (только для зарегистрированных пользователей)
                var catalogs = await _context.Catalogs
                    .Where(c => c.Author == user.Name) // Только для зарегистрированных пользователей
                    .ToListAsync();

                userWithCatalogs.Add(new UserWithCatalogsViewModel
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    ProfileImageUrl = user.ProfileImageUrl,
                    Catalogs = catalogs
                });
            }

            // Получаем каталоги только для анонимных пользователей
            var anonymousCatalogs = await _context.Catalogs
                .Where(c => c.Author == "Anonymous") // Только для анонимных
                .ToListAsync();

            var anonymousCatalogsViewModel = new UserWithCatalogsViewModel
            {
                UserId = -1, // Для анонимных пользователей
                Name = "Anonymous", // Имя для анонимных
                Catalogs = anonymousCatalogs
            };

            // Добавляем анонимные каталоги в отдельный блок
            var allUsersWithCatalogs = new List<UserWithCatalogsViewModel>(userWithCatalogs);
            allUsersWithCatalogs.Add(anonymousCatalogsViewModel);

            return View(allUsersWithCatalogs);  // Возвращаем список пользователей с их каталогами (отдельно для анонимов)
        }
    }
}
