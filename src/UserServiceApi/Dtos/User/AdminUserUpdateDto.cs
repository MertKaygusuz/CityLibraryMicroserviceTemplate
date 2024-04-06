using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserServiceApi.Dtos.User
{
    public class AdminUserUpdateDto : UserSelfUpdateDto
    {
        [Key]
        [DisplayName("User")]
        public string UserId { get; set; } // Used for update
    }
}