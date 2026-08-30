using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace AuthService.API.OpenApi;

/// <summary>
/// .NET 9's AddOpenApi() generates the document without any security scheme, so Swagger UI shows
/// no "Authorize" button and [Authorize] endpoints cannot be exercised by hand.
/// </summary>
public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    private const string SchemeName = "Bearer";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Paste the access token returned by /api/v1/auth/login.",
        };

        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = SchemeName },
            }] = [],
        });

        return Task.CompletedTask;
    }
}
