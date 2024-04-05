using Microsoft.EntityFrameworkCore;
using UserServiceApi.Entities;

namespace UserServiceApi.ContextRelated
{
    public partial class AppDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }

        public DbSet<Roles> Roles { get; set; }

        public DbSet<UserRoles> UserRoles { get; set; }
    }
}
