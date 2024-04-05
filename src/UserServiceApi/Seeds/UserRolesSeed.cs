using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserServiceApi.Entities;


namespace UserServiceApi.Seeds
{
    class UserRolesSeed : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder.HasData(
                new UserRoles
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                },
                new UserRoles
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                },
                 new UserRoles
                 {
                     Id = 3,
                     RoleId = 2,
                     UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                 },
                 new UserRoles
                 {
                     Id = 4,
                     RoleId = 2,
                     UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                 },
                 new UserRoles
                 {
                     Id = 5,
                     RoleId = 2,
                     UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                 }
            );
        }
    }
}
