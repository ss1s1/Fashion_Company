namespace Fashion_Company.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Инициализируем пустой строкой
        public string Description { get; set; } = string.Empty; // Инициализируем пустой строкой
        public string ImageUrl { get; set; } = string.Empty; // Инициализируем пустой строкой
        public string Author { get; set; } = "Anonymous"; // Автор
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Дата создания
        public string? Editor { get; set; } // Последний редактор
        public int Likes { get; set; } = 0;
    }


}
