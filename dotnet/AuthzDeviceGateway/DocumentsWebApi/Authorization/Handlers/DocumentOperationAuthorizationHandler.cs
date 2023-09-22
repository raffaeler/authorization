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
        //context.Succeed(requirement);
        //return Task.CompletedTask;

        var documentOperationClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimNames.DocsLcrud);

        if (documentOperationClaim != null)
        {
            var user = context.User;

            var operations = Operations.GetAllowedOperations(documentOperationClaim.Value);
            if (operations.Contains(requirement))
            {
                _logger.LogInformation($"Requirement succeeded - User:{user}");
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogInformation($"Requirement not met - User:{user}");
            }
        }

        return Task.CompletedTask;
    }

}
