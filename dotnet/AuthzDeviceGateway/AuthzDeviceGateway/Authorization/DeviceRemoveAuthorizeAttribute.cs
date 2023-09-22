using Microsoft.AspNetCore.Authorization;

namespace AuthzDeviceGateway.Authorization;

public class DeviceRemoveAuthorizeAttribute:
    AuthorizeAttribute,
    IAuthorizationRequirement,
    IAuthorizationRequirementData
{
    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
