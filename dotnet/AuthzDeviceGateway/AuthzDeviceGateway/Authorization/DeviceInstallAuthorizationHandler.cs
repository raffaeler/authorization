using Microsoft.AspNetCore.Authorization;

namespace AuthzDeviceGateway.Authorization;

public class DeviceInstallAuthorizationHandler :
    AuthorizationHandler<DeviceInstallAuthorizeAttribute>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeviceInstallAuthorizeAttribute requirement)
    {
        throw new NotImplementedException();
    }
}
