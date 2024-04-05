using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserServiceApi.Entities;

namespace UserServiceApi.Seeds
{
    class RolesSeed : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            Roles[] roles =
            [
                new Roles { RoleId = 1, RoleName = "Admin" },
                new Roles { RoleId = 2, RoleName = "User"}
            ];
            builder.HasData(roles);
        }
    }
}
