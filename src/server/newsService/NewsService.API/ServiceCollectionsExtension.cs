using System.Text.Json;
using System.Text.Json.Serialization;

namespace NewsService.API;

internal static class ServiceCollectionsExtension
{
    
    public static IServiceCollection AddCustomControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                //TODO: SergeyV - Wait till AuthorizeFilter will be added to release version
                //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1295
                //options.Filters.Add(new AuthorizeFilter());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ApplyDefaultSettings();
            });

        return services.AddResponseCompression(options => { options.EnableForHttps = true; });
    }
        
    private static JsonSerializerOptions ApplyDefaultSettings(this JsonSerializerOptions options)
    {
        JsonConverter enumConverter = new JsonStringEnumConverter();
        options.Converters.Add(enumConverter);

        return options;
    }
}