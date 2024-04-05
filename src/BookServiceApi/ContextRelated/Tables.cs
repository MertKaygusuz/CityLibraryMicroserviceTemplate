using BookServiceApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookServiceApi.ContextRelated
{
    public partial class AppDbContext : DbContext
    {
        public DbSet<Books> Books { get; set; }
        
        public DbSet<Users> Users { get; set; }
    }
}