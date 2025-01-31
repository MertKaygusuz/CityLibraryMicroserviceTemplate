using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BookServiceApi.ContextRelated;
using BookServiceApi.IntegrationTests.Helpers;

namespace BookServiceApi.IntegrationTests.ServicesExtensions;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<AppDbContext>));

        if (descriptor != null)
            services.Remove(descriptor);
    }

    public static void EnsureCreated<T>(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<AppDbContext>();

        db.Database.Migrate();
        // DbHelpers.InitDbForTests(db);
    }
}