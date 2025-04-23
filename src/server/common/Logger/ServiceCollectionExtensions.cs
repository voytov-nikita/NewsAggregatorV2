using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Serilog;

namespace Logger;

public static class ServiceCollectionExtensions
{
    public static ConfigureHostBuilder AddLogger(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddLoggerJsonFile(context.HostingEnvironment.EnvironmentName,
                    context.HostingEnvironment.ContentRootPath);
            })
            .UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

        return hostBuilder;
    }

    private static IConfigurationBuilder AddLoggerJsonFile(this IConfigurationBuilder builder, string environmentName,
        string rootPath)
    {
        string newBaseDirectory = rootPath;

        //Workaround: Added to support in IDE. Due to configuration files will be coped to out put folder, but for debugger base path will follow to root folder of project.
        string location = typeof(ServiceCollectionExtensions).Assembly.Location;
        if (!string.IsNullOrEmpty(location))
        {
            newBaseDirectory = Path.GetDirectoryName(location);
        }

        builder.SetBasePath(newBaseDirectory);

        //builder.InsetJson($"serilogsettings.{environmentName}.json", true);
        builder.InsertJson($"serilogsettings.json");

        return builder;
    }
    
    
    private static IConfigurationBuilder InsertJson(this IConfigurationBuilder builder, string path, bool optional = false, int index = 0)
    {
        builder.Sources.Insert(index, new JsonConfigurationSource
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = true
        });
        return builder;
    }
}