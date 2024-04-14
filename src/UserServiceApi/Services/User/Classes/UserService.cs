using static CityLibrary.Shared.Extensions.TokenExtensions.AccesInfoFromToken;
using Microsoft.EntityFrameworkCore;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.ExceptionHandling;
using CityLibrary.Shared.MapperConfigurations;
using Microsoft.Extensions.Localization;
using UserServiceApi.Dtos.User;
using UserServiceApi.Entities;
using UserServiceApi.Extensions;
using UserServiceApi.Repositories;
using UserServiceApi.Resources;
using UserServiceApi.Services.User.Interfaces;
using MassTransit;
using CityLibrary.Shared.SharedModels.QueueModels;
using UserServiceApi.AppSettings;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace UserServiceApi.Services.User.Classes
{
    public class UserService : IUserService
    {
        private readonly IUsersRepo _usersRepo;
        private readonly IRolesRepo _rolesRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IOptions<AppSetting> _options;
        private readonly IStringLocalizer<ExceptionsResource> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HashSet<string> _defaultUserRoleNames = ["User"];
        // used for unit test structure
        public virtual Func<AdminUserUpdateDto, Task> AdminUpdateUserFunc {get; set;}

        public UserService(IUsersRepo usersRepo, 
                           IRolesRepo rolesRepo,
                           IStringLocalizer<ExceptionsResource> localizer,
                           IUnitOfWork unitOfWork,
                           IOptions<AppSetting> options,
                           ISendEndpointProvider sendEndpointProvider,
                           IPublishEndpoint publishEndpoint,
                           ICustomMapper mapper,
                           IHttpContextAccessor httpContextAccessor)
        {
            _usersRepo = usersRepo;
            _rolesRepo = rolesRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _publishEndpoint = publishEndpoint;
            _options = options;
            _sendEndpointProvider = sendEndpointProvider;
            _httpContextAccessor = httpContextAccessor;
            AdminUpdateUserFunc = AdminUpdateUserAsync;
        }
        public async Task<Entities.User> GetUserByUserNameAsync(string userName)
        {
            return await _usersRepo.GetDataWithLinqExp(x => x.UserName == userName, "Roles")
                                     .SingleOrDefaultAsync();
        }

        public async Task<string> RegisterAsync(RegistrationDto registrationDto)
        {
            Entities.User newUser = _mapper.Map<RegistrationDto, Entities.User>(registrationDto);
            newUser.Password.CreatePasswordHash(out string hashedPass);
            newUser.Password = hashedPass;
            newUser.UserId = Guid.NewGuid().ToString();

            _rolesRepo.SetUserRolesWithLinqExp(newUser ,x => _defaultUserRoleNames.Contains(x.RoleName));

            await _usersRepo.InsertAsync(newUser);

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_options.Value.RabbitMqOptions.UserCreatedSenderUri));
            await sendEndpoint.Send(_mapper.Map<Entities.User, UserCreated>(newUser));

            await _unitOfWork.CommitAsync();

            return newUser.UserId;
        }

        public async Task<bool> DoesEntityExistAsync(IConvertible id)
        {
            return await _usersRepo.DoesEntityExistAsync(id as string);
        }

        public async Task UserSelfUpdateAsync(UserSelfUpdateDto selfUpdateDto)
        {
            string myUserId = _httpContextAccessor?.HttpContext?.User.Claims.Where(x => x.Type == ClaimTypes.Sid)
                                                                            .Select(x => x.Value)
                                                                            .FirstOrDefault();
            if (string.IsNullOrEmpty(myUserId) || !await _usersRepo.DoesEntityExistAsync(myUserId))
                throw new CustomBusinessException(_localizer["User_Not_Found"]);

            var updateDto = _mapper.Map<UserSelfUpdateDto, AdminUserUpdateDto>(selfUpdateDto);
            updateDto.UserId = myUserId;
            await AdminUpdateUserFunc(updateDto);
        }

        public virtual async Task AdminUpdateUserAsync(AdminUserUpdateDto updateDto)
        {
            Entities.User theUser = await _usersRepo.GetByIdAsync(updateDto.UserId);
            theUser.FullName = updateDto.FullName;
            theUser.BirthDate = updateDto.BirthDate;
            theUser.Address = updateDto.Address;
            updateDto.Password.CreatePasswordHash(out string hashedPass);
            theUser.Password = hashedPass;

            await _publishEndpoint.Publish(_mapper.Map<Entities.User, UserUpdated>(theUser));

            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> CheckIfExistWithUserNameAsync(string userName)
        {
            return await _usersRepo.GetDataWithLinqExp(x => x.UserName == userName).AnyAsync();
        }
    }
}
