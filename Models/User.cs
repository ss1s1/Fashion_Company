using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Fashion_Company.Models

{
    public class User
    {
        public int Id { get; set; } // Уникальный идентификатор

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty; // Инициализируем пустой строкой // Имя пользователя
        
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty; // Инициализируем пустой строкой // Почта

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string PasswordHash { get; set; } = string.Empty; // Хеш пароля
        public string? Token { get; set; } // Токен для аутентификации (может быть NULL)
        public string? ProfileImageUrl { get; set; } // Ссылка на изображение (может быть NULL)
        public string Role { get; set; } = "User"; // Роль пользователя: "Admin" или "User"
    }

}
