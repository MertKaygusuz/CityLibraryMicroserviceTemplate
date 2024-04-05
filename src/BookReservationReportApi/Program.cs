using System.Text.Json.Serialization;
using BookReservationReportApi.AppSettings;
using BookReservationReportApi.GrpcServices.ReservationReport;
using BookReservationReportApi.ServicesExtensions;
using CityLibrary.Shared.ExceptionHandling.Extensions;
using CityLibrary.Shared.Extensions.TokenExtensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(builder.Configuration);
});

var configuration = builder.Configuration;
// IWebHostEnvironment environment = builder.Environment;

builder.Services.Configure<AppSetting>(configuration);
var appSetting = configuration.Get<AppSetting>();

builder.Services.AddCustomLocalizationConfiguration();

ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Continue;
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AppSetting>();

builder.Services.AddRangeCustomServices(appSetting);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddCustomSwaggerConfiguration();

builder.Services.AddCors();
builder.Services.AddGrpc();

builder.Services.AddHealthChecks();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CityLibrary Book Reservation Report Api v1"));
}

app.UseSerilogRequestLogging();

app.UseRequestLocalization();

app.UseStaticHttpContext();

app.UseCors(opts =>
        // Content dispositon is useful for getting file name for frontend application
        opts.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Content-Disposition")
);

app.UseCustomGlobalExceptionHandler();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapGrpcService<GrpcReservationRecordService>();

app.MapHealthChecks("/healthz");

app.Run();

