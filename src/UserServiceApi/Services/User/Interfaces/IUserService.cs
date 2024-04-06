using CityLibrary.Shared.BaseCheckService;
using UserServiceApi.Dtos.User;
using UserServiceApi.Entities;

namespace UserServiceApi.Services.User.Interfaces
{
    public interface IUserService : IBaseCheckService
    {
        Task<Users> GetUserByUserNameAsync(string userName);

        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="registrationDto"></param>
        /// <returns>User Id</returns>
        Task<string> RegisterAsync(RegistrationDto registrationDto);

        /// <summary>
        /// User can be update its own information by using this method.
        /// </summary>
        /// <param name="selfUpdateDto"></param>
        /// <returns></returns>
        Task UserSelfUpdateAsync(UserSelfUpdateDto selfUpdateDto);

        /// <summary>
        /// Updates any user's information. Only admin must be allowed to run this method.
        /// </summary>
        /// <param name="registrationDto">Includes UserName and update parameters. UserName could not be updated.</param>
        /// <returns></returns>
        Task AdminUpdateUserAsync(AdminUserUpdateDto updateDto);
    }
}
