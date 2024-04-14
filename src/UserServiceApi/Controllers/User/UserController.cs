using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserServiceApi.ActionFilters.Base;
using UserServiceApi.ActionFilters.Interfaces;
using UserServiceApi.Dtos.User;
using UserServiceApi.Services.User.Interfaces;

namespace UserServiceApi.Controllers.User
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [ServiceFilter(typeof(IUserNameCheckFilter))]
        [AllowAnonymous]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<string> Register(RegistrationDto dto)
        {
            return await _userService.RegisterAsync(dto);
        }

        [HttpPut]
        [ServiceFilter(typeof(GenericNotFoundFilter<IUserService>))]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AdminUpdateUser(AdminUserUpdateDto dto)
        {
            await _userService.AdminUpdateUserAsync(dto);
            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UserSelfUpdate(UserSelfUpdateDto dto)
        {
            await _userService.UserSelfUpdateAsync(dto);
            return NoContent();
        }
    }
}
