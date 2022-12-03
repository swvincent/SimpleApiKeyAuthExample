using Microsoft.AspNetCore.Authentication;
using SimpleApiKeyAuthExample.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiKeyExtensions
{
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
    {
        return builder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
            ApiKeyDefaults.AuthenticationScheme,
            options => { }
        );
    }
}