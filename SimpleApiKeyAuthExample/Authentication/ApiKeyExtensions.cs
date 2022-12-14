using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using SimpleApiKeyAuthExample.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiKeyExtensions
{
    /// <summary>
    /// Add security definition for API Key
    /// </summary>
    /// <param name="services"></param>
    public static void AddApiKeySwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                Description = "API Key must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = ApiKeyDefaults.ApiKeyHeader,
                In = ParameterLocation.Header,
                Scheme = "ApiKeyScheme"
            });

            var key = new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            };

            var requirement = new OpenApiSecurityRequirement
            {
                { key, new List<string>() }
            };

            c.AddSecurityRequirement(requirement);
        });
    }

    /// <summary>
    /// Add API Key authentication
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
            ApiKeyDefaults.AuthenticationScheme,
            options => { }
        );
    }
}