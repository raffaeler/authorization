using System.Security.Claims;

using AuthzForm.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace AuthzForm.Authorization.Handlers;

// Handlers for the same Requirement are combined in OR
public class DeveloperRequirementHandler : AuthorizationHandler<TechStaffRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TechStaffRequirement requirement)
    {
        var isDeveloper = context.User.Claims
            .Any(c => c.Type == AuthorizationHelpers.DepartmentClaimType &&
                string.Compare(c.Value, "Developer",
                    StringComparison.InvariantCultureIgnoreCase) == 0);

        if (isDeveloper) context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
