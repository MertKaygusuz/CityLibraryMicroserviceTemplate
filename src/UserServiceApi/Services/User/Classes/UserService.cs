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
        private readonly HashSet<string> _defaultUserRoleNames = ["User"];
        public UserService(IUsersRepo usersRepo, 
                           IRolesRepo rolesRepo,
                           IStringLocalizer<ExceptionsResource> localizer,
                           IUnitOfWork unitOfWork,
                           IOptions<AppSetting> options,
                           ISendEndpointProvider sendEndpointProvider,
                           IPublishEndpoint publishEndpoint,
                           ICustomMapper mapper)
        {
            _usersRepo = usersRepo;
            _rolesRepo = rolesRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _publishEndpoint = publishEndpoint;
            _options = options;
            _sendEndpointProvider = sendEndpointProvider;
        }
        public async Task<Users> GetUserByUserNameAsync(string userName)
        {
            return await _usersRepo.GetDataWithLinqExp(x => x.UserName == userName, "Roles")
                                     .SingleOrDefaultAsync();
        }

        public async Task<string> RegisterAsync(RegistrationDto registrationDto)
        {
            Users newUser = _mapper.Map<RegistrationDto, Users>(registrationDto);
            newUser.Password.CreatePasswordHash(out string hashedPass);
            newUser.Password = hashedPass;
            newUser.UserId = Guid.NewGuid().ToString();

            var roles = _rolesRepo.GetLocalViewWithLinqExp(x => _defaultUserRoleNames.Contains(x.RoleName));
            newUser.Roles = [.. roles];

            await _usersRepo.InsertAsync(newUser);

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(_options.Value.RabbitMqOptions.UserCreatedSenderUri));
            await sendEndpoint.Send(_mapper.Map<Users, UserCreated>(newUser));

            await _unitOfWork.CommitAsync();

            return newUser.UserId;
        }

        public async Task<bool> DoesEntityExistAsync(IConvertible id)
        {
            return await _usersRepo.DoesEntityExistAsync(id as string);
        }

        public async Task UserSelfUpdateAsync(UserSelfUpdateDto selfUpdateDto)
        {
            string myUserId = GetMyUserId();
            if (string.IsNullOrEmpty(myUserId) || !await _usersRepo.DoesEntityExistAsync(myUserId))
                throw new CustomBusinessException(_localizer["User_Not_Found"]);

            var registrationDto = _mapper.Map<UserSelfUpdateDto, RegistrationDto>(selfUpdateDto);
            registrationDto.UserId = myUserId;
            await AdminUpdateUserAsync(registrationDto);
        }

        public async Task AdminUpdateUserAsync(RegistrationDto registrationDto)
        {
            Users theUser = await _usersRepo.GetByIdAsync(registrationDto.UserId);
            theUser.FullName = registrationDto.FullName;
            theUser.BirthDate = registrationDto.BirthDate;
            theUser.Address = registrationDto.Address;
            registrationDto.Password.CreatePasswordHash(out string hashedPass);
            theUser.Password = hashedPass;

            await _publishEndpoint.Publish(_mapper.Map<RegistrationDto, UserUpdated>(registrationDto));

            await _unitOfWork.CommitAsync();
        }
    }
}
