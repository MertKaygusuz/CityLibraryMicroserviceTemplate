using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Entities
{
    public class UserRoles : TableBase
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public Users User { get; set; }

        public int RoleId { get; set; }

        public Roles Role { get; set; }
    }
}
