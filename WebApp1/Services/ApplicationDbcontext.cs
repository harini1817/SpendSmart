using Microsoft.EntityFrameworkCore;
using WebApp1.Models;

namespace WebApp1.Services
{
    public class ApplicationDbcontext : DbContext
    {
        public ApplicationDbcontext(DbContextOptions options) : base(options) {
        }
        public DbSet<Product> Products {get; set;}
    }
}
