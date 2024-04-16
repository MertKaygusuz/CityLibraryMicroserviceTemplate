using static CityLibrary.Shared.Statics.Methods.TokenRelated;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using NetCore.AutoRegisterDi;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using UserServiceApi.AppSettings;
using BookServiceApi.ContextRelated;
using CityLibrary.Shared.MapperConfigurations;
using CityLibrary.Shared.DbBase.SQL.UnitOfWorks;
using CityLibrary.Shared.DbBase.SQL;
using BookServiceApi.Repositories.Base;
using CityLibrary.Shared.SwaggerRelated;
using BookServiceApi.ActionFilters.Base;
using BookServiceApi.UnitOfWorks;
using Refit;
using BookServiceApi.Services.BookReservationApiService;
using System.Text.Json;
using MassTransit;
using BookServiceApi.Consumers;
using BookServiceApi.Repositories.User;
using BookServiceApi.Services.BookReservationApiService.Grpc;

namespace BookServiceApi.ServicesExtensions
{
    public static class ServicesExtensions
    {
        public static void AddRangeCustomServices(this IServiceCollection services, AppSetting appSetting)
        {
            services.AddCustomJwtService(appSetting.TokenOptions);
            // services.AddCustomGeneralAuthorization(appSetting.TokenOptions);
            services.AddHttpContextAccessor();

            services.AddDbContext<AppDbContext>(options =>
            {
                var triggerAssembly = Assembly.GetAssembly(typeof(AppDbContext));
                options.UseTriggers(triggerOptions => triggerOptions.AddAssemblyTriggers(triggerAssembly!));
                options.UseInMemoryDatabase(appSetting.DbConnectionString);
            });

            services.AddSingleton<ICustomMapper, MapsterMapping>();
            services.AddSingleton<IBookReservationRecordApiGrpc, BookReservationRecordApiGrpc>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddScoped(typeof(IBaseRepo<TableBase, IConvertible>), typeof(BaseRepo<TableBase, IConvertible>));

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

            services.AddScoped(x => new Lazy<IUsersRepo>(x.GetService<IUsersRepo>));
        }

        private static void AddTransientServices(this IServiceCollection services, Assembly[] assembliesToScan)
        {
            services.AddTransient(typeof(GenericNotFoundFilter<>));
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
                // opts.UseSecurityTokenValidators = true;
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

        // private static void AddCustomGeneralAuthorization(this IServiceCollection services, TokenOptions tokenOptions)
        // {
        //     // const string issuerKey = "iss";
        //     // const string auidienceKey = "aud";
            
        //     // services.AddAuthorization(options =>
        //     // {
        //     //     options.AddPolicy("TokenKeysPolicy",
        //     //         policy => policy.RequireClaim(issuerKey, tokenOptions.Issuer)
        //     //                                      .RequireClaim(auidienceKey, tokenOptions.Audience));
        //     // });

        // }

        public static void AddCustomSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityLibrary_Book", Version = "v1" });
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
                // c.AddSecurityRequirement(new OpenApiSecurityRequirement
                // {
                //     {
                //         new OpenApiSecurityScheme
                //         {
                //             Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                //         },
                //         new string[] { }
                //     }
                // });
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

        public static void BuildHttpClients(this IServiceCollection services, AppSetting appSetting)
        {
            HttpClient reservationApiClient = new()
            {
                BaseAddress = new Uri(appSetting.ReservationServiceBaseUrl)
            };

            var options = new JsonSerializerOptions();

            services.AddHttpClient("BookReservationRecordApiClient")
                    .AddTypedClient(x => RestService.For<IBookReservationRecordApi>(reservationApiClient, new RefitSettings()
                    {
                        ContentSerializer = new SystemTextJsonContentSerializer(options)
                    }));
        }

        public static void AddRabbitMqMassTransitConfiguration(this IServiceCollection services, RabbitMq rabbitMqOptions)
        {
            services.AddMassTransit(x =>
            {
                x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("book-service", false));

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

                    cfg.ReceiveEndpoint(rabbitMqOptions.UserCreatedConsumerUri, e =>
                    {
                        e.ConfigureConsumer<UserCreatedCommandConsumer>(context);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
