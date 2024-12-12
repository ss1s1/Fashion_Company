using Fashion_Company.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fashion_Company.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Catalog> Catalogs { get; set; }

        public new DbSet<User> Users { get; set; }

        public DbSet<Likes> Likes { get; set; }

        
    }

}

