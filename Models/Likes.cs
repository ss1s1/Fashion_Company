namespace Fashion_Company.Models
{
    public class Likes
    {
        public int Id { get; set; } // Идентификатор лайка
        public int CatalogId { get; set; } // Внешний ключ для Catalog
        public Catalog Catalog { get; set; } = new Catalog();  // Навигационное свойство для связи с каталогом
        public int Count { get; set; } // Количество лайков

    }
}
