using System.Security.Claims;

using AuthzForm.Authorization.Requirements;

using Microsoft.AspNetCore.Authorization;

namespace AuthzForm.Authorization.Handlers;

public class SeniorRequirementHandler : AuthorizationHandler<SeniorRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, SeniorRequirement requirement)
    {
        var yearsOfExperience = context.User.Claims
            .FirstOrDefault(c => c.Type == MyClaimNames.YearsOfExperienceClaimType &&
                c.ValueType == ClaimValueTypes.Integer);
        if (yearsOfExperience == null) return Task.CompletedTask;

        // exception in parsing will make this requirement fail
        var years = int.Parse(yearsOfExperience.Value);

        if (years >= requirement.Years) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
