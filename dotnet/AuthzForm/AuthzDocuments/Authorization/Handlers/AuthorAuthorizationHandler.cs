using System.Security.Claims;

using AuthzDocuments.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AuthzDocuments.Authorization.Handlers;

public class AuthorAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Document>
{
    public AuthorAuthorizationHandler()
    {
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Document resource)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;

        if (context.User.Identity == null ||
            resource == null) return Task.CompletedTask;

        var email = ((ClaimsIdentity)context.User.Identity)
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
        if (email == null) return Task.CompletedTask;

        if (string.Compare(resource.Author, email, true) == 0)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

