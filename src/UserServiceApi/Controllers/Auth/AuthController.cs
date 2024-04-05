using Microsoft.AspNetCore.Mvc;
using UserServiceApi.ActionFilters.Interfaces;
using UserServiceApi.Dtos.Authentication;
using UserServiceApi.Dtos.Token.Records;
using UserServiceApi.Services.Token.Interfaces;

namespace UserServiceApi.Controllers.Auth
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReturnTokenRecord), StatusCodes.Status200OK)]
        public async Task<ReturnTokenRecord> Login(LoginDto dto)
        {
            return await _authenticationService.LoginAsync(dto);
        }

        [HttpPost]
        public async Task Logout([FromForm] string refreshToken)
        {
            await _authenticationService.LogoutAsync(refreshToken);
        }

        [HttpPut]
        [ServiceFilter(typeof(IRefreshLoginFilter))]
        [ProducesResponseType(typeof(ReturnTokenRecord), StatusCodes.Status200OK)]
        public async Task<ReturnTokenRecord> ReLoginWithRefreshToken([FromForm] string refreshToken)
        {
            return await _authenticationService.RefreshLoginTokenAsync(refreshToken);
        }
    }
}
