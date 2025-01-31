﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserServiceApi.Entities;


namespace UserServiceApi.Seeds
{
    class UserRolesSeed : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasData(
                new UserRole
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                    CreatedAt = DateTime.UtcNow
                },
                new UserRole
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                    CreatedAt = DateTime.UtcNow
                },
                 new UserRole
                 {
                    Id = 3,
                    RoleId = 2,
                    UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                    CreatedAt = DateTime.UtcNow
                 },
                 new UserRole
                 {
                    Id = 4,
                    RoleId = 2,
                    UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                    CreatedAt = DateTime.UtcNow
                 },
                 new UserRole
                 {
                    Id = 5,
                    RoleId = 2,
                    UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                    CreatedAt = DateTime.UtcNow
                 }
            );
        }
    }
}
