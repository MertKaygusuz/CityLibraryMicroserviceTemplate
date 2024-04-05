using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using UserServiceApi.ContextRelated;

namespace UserServiceApi.UnitOfWorks
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public void Commit()
        {
            context.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
