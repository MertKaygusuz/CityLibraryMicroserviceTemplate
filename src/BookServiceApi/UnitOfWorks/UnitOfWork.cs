using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using BookServiceApi.ContextRelated;

namespace BookServiceApi.UnitOfWorks
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