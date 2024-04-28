using CityLibrary.Shared.DbBase.SQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;


namespace UserServiceApi.ContextRelated
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            // Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Model.GetEntityTypes()
                           .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                           .ToList()
                           .ForEach(entityType =>
                           {
                               builder.Entity(entityType.ClrType)
                                      .HasQueryFilter(ConvertFilterExpressionForSoftDeletables(x => x.DeletedAt == null, entityType.ClrType));
                           });

            base.OnModelCreating(builder);
        }


        private static LambdaExpression ConvertFilterExpressionForSoftDeletables(
                            Expression<Func<ISoftDeletable, bool>> filterExpression,
                            Type entityType)
        {
            var newParam = Expression.Parameter(entityType);
            var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);

            return Expression.Lambda(newBody, newParam);
        }
    }
}
