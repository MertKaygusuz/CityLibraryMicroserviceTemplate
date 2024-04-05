using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserServiceApi.Dtos.User
{
    public class RegistrationDto
    {
        [Key]
        [DisplayName("User")]
        public string UserId { get; set; } // Used for update

        public string UserName { get; set; } // Used for only create

        public string FullName { get; set; }

        public DateTime BirthDate { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }
    }
}
