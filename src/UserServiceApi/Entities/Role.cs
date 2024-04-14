using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Entities
{
    public class Role : TableBase
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        //virtual, might be lazy loaded
        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
