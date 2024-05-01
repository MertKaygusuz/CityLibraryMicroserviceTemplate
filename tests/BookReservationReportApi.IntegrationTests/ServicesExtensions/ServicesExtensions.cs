using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace BookReservationReportApi.IntegrationTests.ServicesExtensions;

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext(this IServiceCollection services) 
    {
        var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(AppDbContext));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    public static void SeedOnInit(this IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();

        using var scope = sp.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<AppDbContext>();

        DbHelpers.InitDbForTests(context);
    }
}