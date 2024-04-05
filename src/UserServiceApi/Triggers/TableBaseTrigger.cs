using CityLibrary.Shared.DbBase.SQL;
using EntityFrameworkCore.Triggered;
using Microsoft.EntityFrameworkCore;
using UserServiceApi.ContextRelated;
using static CityLibrary.Shared.Extensions.TokenExtensions.AccesInfoFromToken;

namespace UserServiceApi.Triggers
{
    public class TableBaseTrigger(AppDbContext dbContext) : IBeforeSaveTrigger<TableBase>
    {
        private readonly AppDbContext _dbContext = dbContext;

        public Task BeforeSave(ITriggerContext<TableBase> context, CancellationToken cancellationToken)
        {
            if (context.ChangeType is ChangeType.Added)
            {
                context.Entity.CreatedAt = DateTime.UtcNow;
                context.Entity.CreatedBy = GetMyUserId();
            }
            else if (context.ChangeType is ChangeType.Modified)
            {
                context.Entity.LastUpdatedAt = DateTime.UtcNow;
                context.Entity.LastUpdatedBy = GetMyUserId();
            }
            else if (context.ChangeType is ChangeType.Deleted)
            {
                context.Entity.DeletedAt = DateTime.UtcNow;
                context.Entity.DeletedBy = GetMyUserId();

                _dbContext.Entry(context.Entity).State = EntityState.Modified;
            }

            return Task.CompletedTask;
        }
    }
}
