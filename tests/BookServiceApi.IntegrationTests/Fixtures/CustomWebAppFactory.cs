using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;
using BookServiceApi.ContextRelated;
using BookServiceApi.IntegrationTests.ServicesExtensions;
using BookServiceApi.IntegrationTests.Mocks;
using BookServiceApi.Services.BookReservationApiService.Grpc;

namespace BookServiceApi.IntegrationTests.Fixtures;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder().Build();
    public async Task InitializeAsync()
    {
        await _mySqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => 
        {
            services.RemoveDbContext<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
            {
                var triggerAssembly = Assembly.GetAssembly(typeof(AppDbContext));
                options.UseTriggers(triggerOptions => triggerOptions.AddAssemblyTriggers(triggerAssembly!));
                options.UseMySql(_mySqlContainer.GetConnectionString(), ServerVersion.AutoDetect(_mySqlContainer.GetConnectionString()));
            });

            services.AddMassTransitTestHarness();
            services.AddSingleton<IBookReservationRecordApiGrpc, BookReservationRecordGrpcMock>();

            services.EnsureCreated<AppDbContext>();
        });
    }

    Task IAsyncLifetime.DisposeAsync() => _mySqlContainer.DisposeAsync().AsTask();
}
