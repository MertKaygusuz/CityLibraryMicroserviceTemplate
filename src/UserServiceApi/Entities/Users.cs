using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Entities
{
    public class Users : TableBase
    {
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }


        public virtual ICollection<UserRoles> UserRoles { get; set; }

        public virtual List<Roles> Roles { get; set; }
    }
}
