using System.Security.Claims;
using System.Text.Json;

using CommonAuth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AuthzDeviceGateway.Authorization;

public class DeviceConfigureAuthorizationHandler :
    AuthorizationHandler<DeviceConfigureAuthorizeAttribute>
{
    private readonly ILogger<DeviceRemoveAuthorizationHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DeviceConfigureAuthorizationHandler(
        ILogger<DeviceRemoveAuthorizationHandler> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// This requirement calls the challenge (redirect to the IP)
    /// to request the claims associated to the 'device.admin' scope
    /// The 'scope' field is filled by the OIDC client in the
    /// OnRedirectToIdentityProvider event handler
    /// </summary>
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DeviceConfigureAuthorizeAttribute requirement)
    {
        _logger.LogInformation($"{nameof(HandleRequirementAsync)}");

        string requiredScope = "device.admin";
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || context.User == null)
            return;

        // read the scope values from the access_token
        // the id_token does not contain the scope claim
        var scopeValues = await TokenHelpers.GetScopesFromAccessToken(httpContext);
        if (!scopeValues.Contains(requiredScope))
        {
            AuthorizationHelper.AddScopes(httpContext, requiredScope);
            await httpContext.ChallengeAsync();
            return;
        }

        if(!context.User.TryGetClaimJsonValue<Dictionary<string, int>>(
            "deviceAdmin", out var properties) || properties == null)
        {
            return;
        }

        if(!properties.TryGetValue("device_configure", out int value))
            return;

        if (value > 0) context.Succeed(requirement);
    }
}
