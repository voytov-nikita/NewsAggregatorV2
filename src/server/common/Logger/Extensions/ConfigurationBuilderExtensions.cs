using Microsoft.Extensions.Configuration.Json;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder InsertJson(this IConfigurationBuilder builder, string path, bool optional = false, int index = 0)
    {
        builder.Sources.Insert(index, new JsonConfigurationSource
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = true,
        });
        return builder;
    }
}
