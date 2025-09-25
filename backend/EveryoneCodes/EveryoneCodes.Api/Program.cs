using EveryoneCodes.Api.Middleware;
using EveryoneCodes.Application;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);


// Option 1: Embedded CSV (default behavior - reads from embedded resource)
//builder.Services.AddCameraInfrastructure(builder.Configuration);

// Option 2: File-based CSV (reads from Data/cameras-defb.csv file)
var solutionRoot = Directory.GetParent(builder.Environment.ContentRootPath)?.FullName ?? builder.Environment.ContentRootPath;
var csvFilePath = Path.Combine(solutionRoot, "Data", "cameras-defb.csv");
builder.Services.AddCameraRepository(csvFilePath);

// Register application services
builder.Services.AddScoped<ICameraService, CameraService>();

const string CorsPolicy = "AngularDev";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: CorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar/v1"));
}

app.UseHttpsRedirection();
app.UseCors("AngularDev");

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();