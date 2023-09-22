using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DocumentsWebApi.Models;

namespace DocumentsWebApi.Authorization.Handlers;

public class DocumentOperationAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement>
{
    private readonly ILogger<DocumentOperationAuthorizationHandler> _logger;

    public DocumentOperationAuthorizationHandler(ILogger<DocumentOperationAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement)
    {
        var docsLcrud = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimNames.DocsLcrud);

        if (docsLcrud != null && context.User.Identity != null)
        {
            var username = context.User.Identity.Name;
            var shortOperation = Operations.GetShortString(requirement);

            if(docsLcrud.Value.Contains(shortOperation,
                StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation($"Requirement succeeded - User:{username} ClaimValue:{docsLcrud.Value} Operation:{requirement.Name}");

                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation($"Requirement not met - User:{username} ClaimValue:{docsLcrud.Value} Operation:{requirement.Name}");
            }
        }

        return Task.CompletedTask;
    }

}
