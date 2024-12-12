using Microsoft.AspNetCore.Mvc;
using Fashion_Company.Data;
using Fashion_Company.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Fashion_Company.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        private string GenerateToken(User user)
        {
            var key = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role), // Добавляем роль
                new Claim("UserId", user.Id.ToString())
            };

            var expiresInHours = _configuration["JwtSettings:ExpiresInHours"];
            if (string.IsNullOrEmpty(expiresInHours) || !int.TryParse(expiresInHours, out var hours))
            {
                throw new InvalidOperationException("JWT expiration time is not configured or invalid.");
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(hours),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal? ValidateToken(string token) // Добавляем "?" для nullable
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "FashionCompany",
                ValidAudience = "FashionCompany",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey"))
            };

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            }
            catch
            {
                return null; // Возвращаем null в случае неудачи
            }
        }


        [HttpPost]
        public IActionResult Register(User user, string confirmPassword, IFormFile profileImage)
        {
            if (ModelState.IsValid)
            {

                // Проверяем совпадение пароля и подтверждения
                if (user.PasswordHash != confirmPassword)
                {
                    ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
                    // Возвращаем ошибку, если данные неверны
                    return BadRequest();
                }

                // Хеширование пароля
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordHash);


                // Сохранение изображения
                if (profileImage != null && profileImage.Length > 0)
                {
                    string uniqueFileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                    string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string filePath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        profileImage.CopyTo(fileStream);
                    }

                    user.ProfileImageUrl = "/images/" + uniqueFileName;
                }

                // Сохранение пользователя в базу
                _context.Users.Add(user);
                _context.SaveChanges();

                // Перенаправление на домашнюю страницу
                return RedirectToAction("Index", "Home");
            }

            // Возвращаем ошибку, если данные неверны
            return BadRequest("Invalid credentials.");
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (result == PasswordVerificationResult.Success)
                {
                    var token = GenerateToken(user);
                    user.Token = token;
                    _context.SaveChanges();
                    // Устанавливаем данные в сессию
                    HttpContext.Session.SetString("Token", token);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.Name);
                    HttpContext.Session.SetString("UserImage", user.ProfileImageUrl ?? "/images/default-avatar.png");

                    // Проверяем, является ли пользователь администратором
                    if (user.Role == "Admin") // Предполагается, что Role хранит строку
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Role, "Admin")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                        HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
                    }

                    return Ok(); // Возвращаем успешный статус
                }
            }

            // Возвращаем ошибку, если данные неверны
            return BadRequest("Invalid credentials.");
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); // Перенаправляем на главную страницу
        }

        [HttpPost]
        public IActionResult ExitAdminMode()
        {
            if (User.IsInRole("Admin"))
            {
                // Удаляем роль "Admin" из утверждений
                var identity = (ClaimsIdentity)User.Identity!;
                var claims = identity.Claims.Where(c => c.Type != ClaimTypes.Role).ToList();

                // Создаем новый ClaimsPrincipal без роли "Admin"
                var newIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(newIdentity);

                // Обновляем сессию пользователя
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }


            return RedirectToAction("Index", "Home"); // Перенаправляем на главную страницу
        }

    }
}


