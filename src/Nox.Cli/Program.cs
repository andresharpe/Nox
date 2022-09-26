﻿
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nox.Cli.Commands;
using Nox.Cli.Services;
using Nox.Dynamic;
using NoxConsole.Configuration;
using Serilog;
using Spectre.Console.Cli;

internal class Program
{
    private static IConfiguration Configuration { get; set; } = null!;

    public static async Task<int> Main(string[] args)
    {

        var hostBuilder = CreateHostBuilder(args);
        var registrar = new TypeRegistrar(hostBuilder);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            // Register available commands
            config.AddCommand<SyncCommand>("sync")
                .WithDescription("Builds database and syncs data.")
                .WithExample(new[] { "sync" });

        });

        return app.Run(args); 

    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        // App Configuration

        Configuration = ConfigurationHelper.GetApplicationConfiguration(args);

        // Logger

        ILogger logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();

        Log.Logger = logger;

        // HostBuilder

        var hostBuilder = Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(Configuration);
            })
            .UseSerilog();

        return hostBuilder;
    }


}




