using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.ExceptionHandling;
using CityLibrary.Shared.MapperConfigurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserServiceApi.Entities;
using UserServiceApi.Repositories;


namespace UserServiceApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IUsersRepo _usersRepo;
        private readonly IRolesRepo _rolesRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomMapper _mapper;
        private readonly IUserRolesRepo _userRolesRepo;
        private readonly HashSet<string> _defaultUserRoleNames = new() { "User" };
        public TestController(IUsersRepo usersRepo, IRolesRepo rolesRepo, IUnitOfWork unitOfWork, ICustomMapper mapper, IUserRolesRepo userRolesRepo)
        {
            _usersRepo = usersRepo;
            _rolesRepo = rolesRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userRolesRepo = userRolesRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            return await _usersRepo.GetData().Include(x => x.UserRoles)
                .Include(x => x.Roles)
                .ToListAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<Roles>> GetAllRoles()
        {
            return await _rolesRepo.GetData().Include(a => a.UserRoles).Include(x => x.Users).IgnoreQueryFilters().ToListAsync();
        }

        [HttpGet]
        public async Task DeleteFirstRole()
        {
            var role = await _rolesRepo.GetData(false).FirstOrDefaultAsync();

            _rolesRepo.Delete(role);

            _unitOfWork.Commit();
        }

        [HttpGet]
        public void ThrowCustomException()
        {
            throw new CustomStatusException("Not found", 404);
        }

        [HttpGet]
        public void ThrowInternalException()
        {
            var a = 1;
            var b = 0;
            var c = a / b;
        }
    }
}
