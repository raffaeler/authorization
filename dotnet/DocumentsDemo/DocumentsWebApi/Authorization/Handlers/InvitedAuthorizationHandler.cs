using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DocumentsWebApi.Models;

namespace DocumentsWebApi.Authorization.Handlers;

public class InvitedAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Document>
{
    private readonly ILogger<InvitedAuthorizationHandler> _logger;

    public InvitedAuthorizationHandler(ILogger<InvitedAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Document document)
    {
        if (context.User.Identity == null ||
            document == null) return Task.CompletedTask;

        var username = context.User.Identity.Name;
        var shortOperation = Operations.GetShortString(requirement);
        var sharedWithList = document.Shares
            .Where(s => s.PermissionSet.Contains(shortOperation))
            .Select(s => s.Username)
            .ToList();

        var email = ((ClaimsIdentity)context.User.Identity)
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
        if (email == null)
        {
            _logger.LogInformation($"Requirement not met (invalid email) - User:{username} Document:{document.Id}");
            return Task.CompletedTask;
        }

        if (sharedWithList.Any(s => string.Compare(s, email, true) == 0))
        {
            _logger.LogInformation($"Requirement succeeded - User:{username} Document:{document.Id}");

            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation($"Requirement not met - User:{username} Document:{document.Id}");
        }

        return Task.CompletedTask;
    }
}
