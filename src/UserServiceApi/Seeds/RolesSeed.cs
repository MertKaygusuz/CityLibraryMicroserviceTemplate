using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserServiceApi.Entities;

namespace UserServiceApi.Seeds
{
    class RolesSeed : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            Role[] roles =
            [
                new Role { RoleId = 1, RoleName = "Admin", CreatedAt = DateTime.UtcNow },
                new Role { RoleId = 2, RoleName = "User", CreatedAt = DateTime.UtcNow }
            ];
            builder.HasData(roles);
        }
    }
}
