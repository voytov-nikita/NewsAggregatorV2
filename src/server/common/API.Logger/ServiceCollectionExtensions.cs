using Logger;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace API.Logger;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder UseLogger(this WebApplicationBuilder builder)
    {
        builder.Host.ConfigureCustomLogger();
        return builder;
    }

    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }
}
