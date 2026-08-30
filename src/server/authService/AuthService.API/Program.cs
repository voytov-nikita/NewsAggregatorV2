using Common.API;
using Common.Auth;
using AuthService.API.OpenApi;
using AuthService.API.Settings;
using AuthService.BLL;
using AuthService.DAL.PostgreSql;

namespace AuthService.API;

public class Program
{
    private const string ClientCorsPolicy = "AllowClient";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // The `local` launch profile sets ASPNETCORE_ENVIRONMENT=Local, so the default
        // user-secrets registration (Development only) never kicks in.
        if (builder.Environment.IsEnvironment("Local"))
        {
            builder.Configuration.AddUserSecrets<Program>(optional: true);
        }

        GlobalSettings globalSettings = builder.Configuration.Get<GlobalSettings>()
                                        ?? throw new InvalidOperationException("Configuration could not be bound to GlobalSettings.");

        builder.ConfigureCommonApiSettings();

        // AllowAnyOrigin() is rejected by browsers when combined with credentials, and the refresh
        // cookie needs credentials - so this service enumerates its origins explicitly.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(ClientCorsPolicy, policy =>
            {
                policy
                    .WithOrigins(globalSettings.Cors.AllowedOrigins)
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddOpenApi(options =>
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());
        builder.Services.AddProblemDetails();

        // Resolved against the content root so the key does not land in bin/ and get wiped by a clean.
        if (!Path.IsPathRooted(globalSettings.Jwt.KeysPath))
        {
            globalSettings.Jwt.KeysPath =
                Path.Combine(builder.Environment.ContentRootPath, globalSettings.Jwt.KeysPath);
        }

        builder.Services.AddSingleton(globalSettings.Cookies);

        // AuthService validates its own tokens as well, so [Authorize] works on /me.
        builder.Services.AddJwtAuthentication(globalSettings.Auth);

        builder.Services.AddBusinessLayer(globalSettings.Jwt);
        builder.Services.AddDataAccessLayer(globalSettings.ConnectionStrings.PostgreSql);

        var app = builder.Build();

        app.UseExceptionHandler();

        // The `local` launch profile sets ASPNETCORE_ENVIRONMENT=Local, so IsDevelopment() alone
        // would be false and Swagger would never be reachable.
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
        {
            app.MapOpenApi();

            app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "AuthService API V1"); });
        }

        app
            .UseHttpsRedirection()
            .UseCors(ClientCorsPolicy)
            .UseRouting()
            // UseCors before auth so a 401 still carries CORS headers; UseRouting before
            // UseAuthorization so endpoint metadata is resolved when authorization runs.
            .UseAuthentication()
            .UseAuthorization()
            .UseResponseCompression();

        app.MapControllers();

        app.Run();
    }
}
