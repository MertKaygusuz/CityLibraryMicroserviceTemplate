using static CityLibrary.Shared.Statics.Methods.TokenRelated;
using System.Globalization;
using CityLibrary.Shared.SwaggerRelated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using BookReservationReportApi.AppSettings;
using System.Reflection;
using NetCore.AutoRegisterDi;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using BookReservationReportApi.ContextRelated;
using Serilog;
using MongoDB.Bson;
using CityLibrary.Shared.MapperConfigurations;
using CityLibrary.Shared.DbBase.Mongo;
using BookReservationReportApi.Repositories.Base;
using MassTransit;
using BookReservationReportApi.Consumers;

namespace BookReservationReportApi.ServicesExtensions
{
    public static class ServicesExtensions
    {
        public static void AddRangeCustomServices(this IServiceCollection services, AppSetting appSetting)
        {
            services.AddCustomJwtService(appSetting.TokenOptions);
            services.AddHttpContextAccessor();

            services.AddSingleton<IMongoClient>(s =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(appSetting.DbConnection.ConnectionString);

                mongoClientSettings.ClusterConfigurator = clusterBuilder =>
                {
                    clusterBuilder.Subscribe<CommandStartedEvent>(e =>
                    {
                        Console.WriteLine($"Command Started: {e.CommandName} - JSON: {e.Command.ToJson()}");
                    });

                    clusterBuilder.Subscribe<CommandSucceededEvent>(e =>
                    {
                        Log.Logger.Information($"Command Succeeded: {e.CommandName}");
                    });
                };

                return new MongoClient(mongoClientSettings);
            });
            services.AddScoped(s => new AppDbContext(s.GetRequiredService<IMongoClient>(), appSetting.DbConnection.DbName));

            services.AddSingleton<ICustomMapper, MapsterMapping>();

            services.AddScoped(typeof(IBaseRepo<TableBase>), typeof(BaseRepo<TableBase>));

            var assembliesToScan = new[]
            {
                Assembly.GetExecutingAssembly()
            };

            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
              .Where(c => c.Name.EndsWith("Repo"))
              .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
              .Where(c => c.Name.EndsWith("Service"))
              .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            services.AddTransientServices(assembliesToScan);
        }
        private static void AddTransientServices(this IServiceCollection services, Assembly[] assembliesToScan)
        {
            // services.AddTransient(typeof(GenericNotFoundFilter<>));
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
              .Where(c => c.Name.EndsWith("Filter"))
              .AsPublicImplementedInterfaces(ServiceLifetime.Transient);
        }
        private static void AddCustomJwtService(this IServiceCollection services, TokenOptions tokenOptions)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience,
                    IssuerSigningKey = GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
        public static void AddCustomSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityLibrary_Book_Reservation_Report", Version = "v1" });
                c.OperationFilter<AcceptLanguageHeaderSwaggerAttribute>();
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, Array.Empty<string>() }
                });
            });
        }

        public static void AddCustomLocalizationConfiguration(this IServiceCollection services)
        {
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(opt =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new ("en-GB"),
                    new ("tr-TR")
                };
                opt.DefaultRequestCulture = new RequestCulture("en-GB");
                opt.SupportedCultures = supportedCultures;
                opt.SupportedUICultures = supportedCultures;

                opt.RequestCultureProviders =
                [
                    new AcceptLanguageHeaderRequestCultureProvider()
                ];
            });
        }

        public static void AddRabbitMqMassTransitConfiguration(this IServiceCollection services, RabbitMq rabbitMqOptions)
        {
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("book-reservation-service", false));

                x.AddConsumersFromNamespaceContaining<UserUpdatedEventConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.UseMessageRetry(r =>
                    {
                        r.Handle<RabbitMqConnectionException>();
                        r.Interval(5, TimeSpan.FromSeconds(10));
                    });

                    cfg.Host(rabbitMqOptions.Host, "/", host =>
                    {
                        host.Username(rabbitMqOptions.UserName);
                        host.Password(rabbitMqOptions.Password);
                    });

                    cfg.ReceiveEndpoint(rabbitMqOptions.BookUpdatedConsumerUri, e =>
                    {
                        e.ConfigureConsumer<BookUpdatedCommandConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}