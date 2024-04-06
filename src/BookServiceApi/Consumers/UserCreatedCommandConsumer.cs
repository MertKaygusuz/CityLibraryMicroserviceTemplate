using BookServiceApi.Entities;
using BookServiceApi.Repositories.User;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.MapperConfigurations;
using CityLibrary.Shared.SharedModels.QueueModels;
using MassTransit;

namespace BookServiceApi.Consumers
{
    public class UserCreatedCommandConsumer(IUsersRepo usersRepo, ICustomMapper mapper, IUnitOfWork unitOfWork) : IConsumer<UserCreated>
    {
        private readonly IUsersRepo _usersRepo = usersRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ICustomMapper _mapper = mapper;

        public async Task Consume(ConsumeContext<UserCreated> context)
        {
            Users newUser = _mapper.Map<UserCreated, Users>(context.Message);
            await _usersRepo.InsertAsync(newUser);
            await _unitOfWork.CommitAsync();
        }
    }
}