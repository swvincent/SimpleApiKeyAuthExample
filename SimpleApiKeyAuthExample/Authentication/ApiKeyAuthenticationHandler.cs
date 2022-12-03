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
        private const string API_KEY_HEADER = "ApiKey";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {}

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(API_KEY_HEADER))
                return Task.FromResult(AuthenticateResult.Fail("Header Not Found."));

            string apiKeyToValidate = Request.Headers[API_KEY_HEADER];

            string apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json")
                .Build().GetSection("AppSettings")[API_KEY_HEADER];

            if (apiKeyToValidate != apiKey)
                return Task.FromResult(AuthenticateResult.Fail("Invalid key."));

            //TODO: Not sure what to do here - creating an arbitrary user for now
            var pskUser = new IdentityUser("PSK");

            return Task.FromResult(AuthenticateResult.Success(CreateTicket(pskUser)));
        }

        private AuthenticationTicket CreateTicket(IdentityUser user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return ticket;
        }
    }
}