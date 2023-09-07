using AuthzForm.Authorization.Requirements;

using Microsoft.AspNetCore.Authorization;

namespace AuthzForm.Authorization.Handlers;

// Handlers for the same Requirement are combined in OR
public class ItproRequirementHandler : AuthorizationHandler<TechStaffRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, TechStaffRequirement requirement)
    {
        var isItpro = context.User.Claims
            .Any(c => c.Type == AuthorizationHelpers.DepartmentClaimType &&
                string.Compare(c.Value, "Itpro",
                    StringComparison.InvariantCultureIgnoreCase) == 0);

        if (isItpro) context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
