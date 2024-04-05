using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookServiceApi.Entities;

namespace BookServiceApi.Seeds
{
    class UsersSeed : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            var members = new Users[]
            {
                new Users
                {
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                    UserName = "Admin",
                    FullName = "Admin",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Address = "Admin's Address",
                    CreatedAt = DateTime.UtcNow
                },
                new Users
                {
                    UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                    UserName = "User1",
                    FullName = "Orhan",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Address = "Orhan's Address",
                    CreatedAt = DateTime.UtcNow
                },
                new Users
                {
                    UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                    UserName = "User2",
                    FullName = "Kaya",
                    BirthDate = DateTime.UtcNow.AddYears(-40),
                    Address = "Kaya's Address",
                    CreatedAt = DateTime.UtcNow
                },
                new Users
                {
                    UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                    UserName = "User3",
                    FullName = "Kadriye",
                    BirthDate = DateTime.UtcNow.AddYears(-20),
                    Address = "Kadriye's Address",
                    CreatedAt = DateTime.UtcNow
                }
            };

            builder.HasData(members);
        }
    }
}
