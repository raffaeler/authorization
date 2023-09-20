using AuthzForm.Authorization.Requirements;

using Microsoft.AspNetCore.Authorization;

namespace AuthzForm.Authorization.Attributes;

public class AuthorizeJuniorsAttribute : AuthorizeAttribute,
    IAuthorizationRequirement,
    IAuthorizationRequirementData
{
    public AuthorizeJuniorsAttribute(int years) => Years = years;

    public int Years { get; }


    public IEnumerable<IAuthorizationRequirement> GetRequirements()
    {
        yield return this;
    }
}
