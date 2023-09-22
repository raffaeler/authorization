using Microsoft.AspNetCore.Authorization;

namespace AuthzDeviceGateway.Authorization;

public class DeviceRemoveAuthorizationHandler :
    AuthorizationHandler<DeviceRemoveAuthorizeAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeviceRemoveAuthorizeAttribute requirement)
    {
        throw new NotImplementedException();
    }
}
