using BookServiceApi.Entities;
using BookServiceApi.Repositories.User;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.SharedModels.QueueModels;
using MassTransit;

namespace BookServiceApi.Consumers
{
    public class UserUpdatedEventConsumer(IUsersRepo usersRepo, IUnitOfWork unitOfWork) : IConsumer<UserUpdated>
    {
        private readonly IUsersRepo _usersRepo = usersRepo;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task Consume(ConsumeContext<UserUpdated> context)
        {
            User theUser = await _usersRepo.GetByIdAsync(context.Message.UserId);
            if (theUser is null) {
                return;
            }

            theUser.FullName = context.Message.FullName;
            theUser.BirthDate = context.Message.BirthDate;
            theUser.Address = context.Message.Address;
            
            await _unitOfWork.CommitAsync();
        }
    }
}