using System.Security.Claims;
using System.Text.Json;

using AuthzForm.Authorization.Requirements;
using AuthzForm.Authorization.Transform;

using Microsoft.AspNetCore.Authorization;

namespace AuthzForm.Authorization.Handlers;

// Handlers for the same Requirement are combined in OR
public class SportRequirementHandler : AuthorizationHandler<SportRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, SportRequirement requirement)
    {
        var authorInfoClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == nameof(AuthorInfo));
        if(authorInfoClaim == null) return Task.CompletedTask;

        var authorInfo = JsonSerializer.Deserialize<AuthorInfo>(authorInfoClaim.Value);
        if(authorInfo == null) return Task.CompletedTask;

        if (authorInfo.Sports.Any()) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
