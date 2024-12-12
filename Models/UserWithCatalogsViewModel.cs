using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Fashion_Company.Data;

namespace Fashion_Company.Models
{
    public class UserWithCatalogsViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

        public List<Catalog> Catalogs { get; set; } = new List<Catalog>();
    }

}
