using Microsoft.AspNetCore.Authorization;

namespace AuthzDeviceGateway.Authorization;

public class DeviceConfigureAuthorizeAttribute :
    AuthorizeAttribute,
    IAuthorizationRequirement,
    IAuthorizationRequirementData
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
