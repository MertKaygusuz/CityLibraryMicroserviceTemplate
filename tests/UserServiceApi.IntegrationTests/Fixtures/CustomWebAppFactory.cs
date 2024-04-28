using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using UserServiceApi.ContextRelated;
using UserServiceApi.IntegrationTests.ServicesExtensions;

namespace UserServiceApi.IntegrationTests.Fixtures;

public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _redisContainer.StartAsync();
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
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
                options.InstanceName = "SampleInstance";
            });

            services.AddMassTransitTestHarness();

            services.EnsureCreated<AppDbContext>();

            // Does not work
            // services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
            //     .AddFakeJwtBearer(opt =>
            //     {
            //         opt.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
            //     });
        });
    }

    async Task IAsyncLifetime.DisposeAsync() 
    {
        await _redisContainer.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }
}
