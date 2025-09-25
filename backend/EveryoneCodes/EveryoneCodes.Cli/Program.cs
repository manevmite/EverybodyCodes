using EveryoneCodes.Application;
using EveryoneCodes.Cli;
using EveryoneCodes.Core.Interfaces;
using EveryoneCodes.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

static string? GetArg(string[] args, params string[] keys)
{
    for (int i = 0; i < args.Length; i++)
    {
        if (keys.Contains(args[i], StringComparer.OrdinalIgnoreCase))
            return (i + 1 < args.Length) ? args[i + 1] : "";
        foreach (var k in keys)
            if (args[i].StartsWith(k + "=", StringComparison.OrdinalIgnoreCase))
                return args[i].Substring(k.Length + 1);
    }
    return null;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("dotnet run -- --name Neude");
}

var name = GetArg(args, "--name", "-n");
if (string.IsNullOrWhiteSpace(name))
{
    PrintUsage();
    return;
}

var builder = Host.CreateApplicationBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.IncludeScopes = true;
    o.SingleLine = true;
    o.TimestampFormat = "HH:mm:ss.fff ";
});
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Option 1: Embedded CSV (default behavior - reads from embedded resource)
// builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
// {
//     ["CameraStore:ResourcePath"] = "Data.cameras-defb.csv",
//     ["CameraStore:EnableCaching"] = "false", // CLI doesn't need caching
//     ["CameraStore:CacheExpiration"] = "00:01:00"
// });
// builder.Services.AddCameraInfrastructure(builder.Configuration);

// Option 2: File-based CSV (reads from Data/cameras-defb.csv file)
var currentDir = Directory.GetCurrentDirectory();
var solutionRoot = Directory.GetParent(currentDir)?.FullName ?? currentDir;
var csvFilePath = Path.Combine(solutionRoot, "Data", "cameras-defb.csv");
builder.Services.AddCameraRepository(csvFilePath);

// App service
builder.Services.AddScoped<ICameraService, CameraService>();

// Command runner
builder.Services.AddTransient<SearchRunner>();

using var host = builder.Build();

var runner = host.Services.GetRequiredService<SearchRunner>();
var exit = await runner.RunAsync(name);
Environment.ExitCode = exit;