using Logger.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Settings.Configuration;

namespace Logger;

public static class ServiceCollectionExtensions
{
    public static IHostBuilder ConfigureCustomLogger(this IHostBuilder hostBuilder)
    {
        return hostBuilder
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddLoggerJsonFile(context.HostingEnvironment.EnvironmentName);
            })
            .ConfigureLogging((context, builder) =>
            {
                ConfigurationReaderOptions readerOptions = new()
                {
                    OnLevelSwitchCreated = (switchName, levelSwitch) =>
                    {
                        if (switchName != "$controlSwitch")
                        {
                            return;
                        }

                        builder.Services.AddSingleton<ILogLevelService>(new LogLevelService(levelSwitch));
                    },
                };

                LoggerConfiguration logConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(context.Configuration, readerOptions);
                Log.Logger = logConfiguration.CreateLogger();

                builder.Services.AddSerilog(Log.Logger);
            });
    }

    private static IConfigurationBuilder AddLoggerJsonFile(this IConfigurationBuilder builder, string environmentName)
    {
        // Workaround: configuration files are copied to the assembly output folder,
        // but the host content root points at the project folder during IDE debug.
        string? assemblyDir = Path.GetDirectoryName(typeof(ServiceCollectionExtensions).Assembly.Location);
        if (!string.IsNullOrEmpty(assemblyDir))
        {
            builder.SetBasePath(assemblyDir);
        }

        builder.InsertJson($"serilogsettings.{environmentName}.json", optional: true);
        builder.InsertJson("serilogsettings.json");

        return builder;
    }
}
