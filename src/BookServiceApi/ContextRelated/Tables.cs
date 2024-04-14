using BookServiceApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookServiceApi.ContextRelated
{
    public partial class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        
        public DbSet<User> Users { get; set; }
    }
}