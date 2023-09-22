using Microsoft.AspNetCore.Authorization;

namespace AuthzDeviceGateway.Authorization;

public class DeviceInstallAuthorizeAttribute :
    AuthorizeAttribute,
    IAuthorizationRequirement,
    IAuthorizationRequirementData
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
