using Microsoft.EntityFrameworkCore;
using UserServiceApi.Entities;

namespace UserServiceApi.ContextRelated
{
    public partial class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
    }
}
