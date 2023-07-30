using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace L1App1.Authentications;

public sealed class DevBearerAuthenticationSchemeHandler : AuthenticationHandler<DevBearerAuthenticationSchemeOptions>
{
    public DevBearerAuthenticationSchemeHandler(
        IOptionsMonitor<DevBearerAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.CompletedTask;
        var apiKeyHeaderValue = Context.Request.Headers["DevBearer"].FirstOrDefault();
        var result = apiKeyHeaderValue switch
        {
            "Guest1" => AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "Guest1"),
                    new Claim(ClaimTypes.Role, "Guest"),
                }, "DevBearer")),
                "DevBearer")),
            "User1" => AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "User1"),
                    new Claim(ClaimTypes.Role, "User"),
                }, "DevBearer")),
                "DevBearer")),
            "User2" => AuthenticateResult.Success(new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "User2"),
                    new Claim(ClaimTypes.Role, "User"),
                }, "DevBearer")),
                "DevBearer")),
            _ => AuthenticateResult.NoResult(),
        };

        return result;
    }
}
