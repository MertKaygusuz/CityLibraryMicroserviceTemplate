using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Entities
{
    public class Roles : TableBase
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        //virtual, might be lazy loaded
        public virtual ICollection<UserRoles> UserRoles { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
