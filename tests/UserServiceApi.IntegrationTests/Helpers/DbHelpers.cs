using Microsoft.EntityFrameworkCore;
using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Extensions;

namespace UserServiceApi.IntegrationTests.Helpers
{
    public static class DbHelpers
    {
        public static void InitDbForTests(AppDbContext db)
        {
            db.Users.AddRange(GetUsersForTest());
            db.UserRoles.AddRange(GetUserRolesForTest());
            db.SaveChanges();
        }

        public static async Task ReinitDbForTests(AppDbContext db)
        {
            await db.UserRoles.ExecuteDeleteAsync();
            await db.Users.ExecuteDeleteAsync();
            db.SaveChanges();
            InitDbForTests(db);
        }

        private static IEnumerable<User> GetUsersForTest()
        {
            string sharedPassword = "1234567890";
            sharedPassword.CreatePasswordHash(out string hashedPass);
            return
            [
                new User
                {
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                    UserName = "Admin",
                    FullName = "Admin",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Address = "Admin's Address",
                    Password = hashedPass
                },
                new User
                {
                    UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                    UserName = "User1",
                    FullName = "Orhan",
                    BirthDate = DateTime.UtcNow.AddYears(-30),
                    Address = "Orhan's Address",
                    Password = hashedPass
                },
                new User
                {
                    UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                    UserName = "User2",
                    FullName = "Kaya",
                    BirthDate = DateTime.UtcNow.AddYears(-40),
                    Address = "Kaya's Address",
                    Password = hashedPass
                },
                new User
                {
                    UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                    UserName = "User3",
                    FullName = "Kadriye",
                    BirthDate = DateTime.UtcNow.AddYears(-20),
                    Address = "Kadriye's Address",
                    Password = hashedPass
                }
            ];
        }

        private static IEnumerable<UserRole> GetUserRolesForTest() 
        {
            return 
            [
                new UserRole
                {
                    Id = 1,
                    RoleId = 1,
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                },
                new UserRole
                {
                    Id = 2,
                    RoleId = 2,
                    UserId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d",
                },
                 new UserRole
                 {
                     Id = 3,
                     RoleId = 2,
                     UserId = "75a4749d-1090-4ade-894e-2612adcd0c1c",
                 },
                 new UserRole
                 {
                     Id = 4,
                     RoleId = 2,
                     UserId = "1146ae0a-cdf3-4822-a691-98f5da9c3f9e",
                 },
                 new UserRole
                 {
                     Id = 5,
                     RoleId = 2,
                     UserId = "739d9fdf-f824-40d8-b909-4586bdc283d3",
                 }
            ];
        }
    }
}