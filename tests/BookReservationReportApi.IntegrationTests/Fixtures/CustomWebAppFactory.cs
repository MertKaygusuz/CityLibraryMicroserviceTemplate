using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.IntegrationTests.ServicesExtensions;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;

namespace BookReservationReportApi.IntegrationTests.Fixtures;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private MongoDbRunner _mongoRunner;
    public Task InitializeAsync()
    {
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: false);
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => 
        {
            services.RemoveDbContext();

            services.AddSingleton<IMongoClient>(s => 
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(_mongoRunner.ConnectionString);
                return new MongoClient(mongoClientSettings);
            });
            services.AddScoped(s => new AppDbContext(s.GetRequiredService<IMongoClient>(), "TestDb"));


            services.AddMassTransitTestHarness();

            services.SeedOnInit();
        });
    }

    Task IAsyncLifetime.DisposeAsync()
    {
        _mongoRunner.Dispose();
        return Task.CompletedTask;
    }
}
