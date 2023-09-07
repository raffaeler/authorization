using Microsoft.AspNetCore.Authorization;

namespace AuthzForm.Authorization.Requirements;

public class SeniorRequirement : IAuthorizationRequirement
{
	public SeniorRequirement(int years)
	{
        Years = years;
    }

    public int Years { get; }
}
