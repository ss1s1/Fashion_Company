using Microsoft.AspNetCore.Mvc;
using Fashion_Company.Data;
using Fashion_Company.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace Fashion_Company.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment; // Для работы с файловой системой
        private readonly ILogger<CatalogController> _logger;
        private static int LikesCount = 0; // Статическая переменная для примера
        public CatalogController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<CatalogController> logger)
        {//механизм Dependency Injection.
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger; // Присваиваем зависимость _logger
        }

        



        public IActionResult Index(string? query)
        {
            var catalogs = _context.Catalogs
                .AsQueryable();

            // Если строка поиска не пуста, фильтруем результаты
            if (!string.IsNullOrWhiteSpace(query))
            {
                catalogs = catalogs.Where(c => c.Name.ToLower().Contains(query.ToLower()) ||
                                               c.Author.ToLower().Contains(query.ToLower()) ||
                                               c.Description.ToLower().Contains(query.ToLower()));
            }

            // Если строка поиска пуста, просто возвращаем все карточки
            else
            {
                catalogs = _context.Catalogs.AsQueryable();
            }

            ViewBag.SearchQuery = query; // Для отображения текущей строки поиска
            ViewBag.CurrentUserName = HttpContext.Session.GetString("UserName"); // Имя текущего пользователя
            return View(catalogs.ToList());
        }

        [HttpPost]
        public IActionResult Create(Catalog catalog, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                // Если есть загружаемое изображение
                if (image != null && image.Length > 0)
                {
                    // Генерация уникального имени файла
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                    // Путь для сохранения изображения
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // Сохранение файла в папку
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }

                    // Сохранение пути к изображению в модель
                    catalog.ImageUrl = "/images/" + uniqueFileName;
                }

                // Добавляем автора
                var userName = HttpContext.Session.GetString("UserName");
                catalog.Author = userName ?? "Anonymous";

                // Добавляем дату создания
                catalog.CreatedDate = DateTime.UtcNow;

                _context.Catalogs.Add(catalog);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Create", catalog);
        }

        [HttpPost]
        public IActionResult Edit(Catalog catalog, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                // Получаем существующий объект из базы данных
                var existingCatalog = _context.Catalogs.FirstOrDefault(c => c.Id == catalog.Id);
                if (existingCatalog == null)
                {
                    return NotFound(); // Каталог с указанным ID не найден
                }

                // Если есть новое изображение, обновляем его
                if (image != null && image.Length > 0)
                {
                    // Генерация уникального имени файла
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                    // Путь для сохранения изображения
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    // Сохранение файла в папку
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(fileStream);
                    }

                    // Обновляем путь к изображению
                    existingCatalog.ImageUrl = "/images/" + uniqueFileName;
                }

                // Сохраняем автора из существующего объекта
                catalog.Author = existingCatalog.Author;

                // Устанавливаем редактора
                string? userName = HttpContext.Session.GetString("UserName");
                existingCatalog.Editor = userName ?? "Anonymous";

                // Обновляем остальные поля
                existingCatalog.Name = catalog.Name;
                existingCatalog.Description = catalog.Description;
                existingCatalog.CreatedDate = existingCatalog.CreatedDate; // Оставляем дату создания неизменной

                // Сохраняем изменения
                _context.Catalogs.Update(existingCatalog);
                _context.SaveChanges();

                return Ok(); // Возвращаем статус успеха
            }

            return BadRequest(); // Возвращаем ошибку, если данные некорректны
        }


        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var catalog = _context.Catalogs.FirstOrDefault(c => c.Id == id);
            if (catalog != null)
            {
                string? currentUserName = HttpContext.Session.GetString("UserName") ?? "Anonymous";

                if (catalog.Author != currentUserName)
                {
                    return Forbid("You are not authorized to delete this catalog."); // Запрещаем удаление
                }

                // Удаляем файл изображения, если он есть
                if (!string.IsNullOrEmpty(catalog.ImageUrl))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, catalog.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Catalogs.Remove(catalog);
                _context.SaveChanges();
                return Ok(); // Возвращаем успешный ответ
            }

            return NotFound(); // Возвращаем ответ, если элемент не найден
        }

        [HttpGet]
        public JsonResult Search(string query)
        {
            var results = _context.Catalogs
                .Where(item => item.Name.Contains(query) ||
                               item.Author.Contains(query) ||
                               item.Description.Contains(query))  // Поиск по имени, автору и описанию
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Author,
                    item.Description,
                    item.ImageUrl,
                    item.CreatedDate,
                    item.Editor
                })
                .Take(10)  // Ограничиваем количество результатов
                .ToList();

            return Json(results);
        }

        public IActionResult CatalogByAuthor(string authorName)
        {
            // Фильтруем каталоги по имени автора
            var catalogs = _context.Catalogs
                .Where(c => c.Author.ToLower() == authorName.ToLower()) // Case-insensitive comparison
                .ToList();

            ViewData["AuthorName"] = authorName;

            if (catalogs.Any())
            {
                return View(catalogs);
            }
            else
            {
                return View("NotFound"); // Страница, если ничего не найдено
            }
        }



        [HttpPost]
        [Route("Catalog/Like/{id}")]
        public IActionResult Like(int id)
        {
            var catalog = _context.Catalogs.FirstOrDefault(c => c.Id == id);
            if (catalog == null)
            {
                return NotFound();
            }

            catalog.Likes += 1; // Увеличиваем количество лайков
            _context.SaveChanges();

            return Ok(new { Likes = catalog.Likes }); // Возвращаем текущее количество лайков
        }






    }
}
