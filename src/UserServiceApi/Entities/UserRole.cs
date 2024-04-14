using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Entities
{
    public class UserRole : TableBase
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public int RoleId { get; set; }

        public Role Role { get; set; }
    }
}
