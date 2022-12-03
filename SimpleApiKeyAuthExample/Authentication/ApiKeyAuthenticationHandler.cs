using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SimpleApiKeyAuthExample.Authentication
{
    class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {}

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(ApiKeyDefaults.ApiKeyHeader))
                return Task.FromResult(AuthenticateResult.Fail("Header Not Found."));

            string apiKeyToValidate = Request.Headers[ApiKeyDefaults.ApiKeyHeader];

            string apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build().GetSection("AppSettings")[ApiKeyDefaults.ApiKeyHeader];

            if (apiKeyToValidate != apiKey)
                return Task.FromResult(AuthenticateResult.Fail("Invalid key."));

            return Task.FromResult(AuthenticateResult.Success(CreateTicket()));
        }

        private AuthenticationTicket CreateTicket()
        {
            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(Array.Empty<Claim>(), this.Scheme.Name));
            
            return new AuthenticationTicket(principal, Scheme.Name);
        }
    }
}