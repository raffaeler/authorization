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

            var operations = GetAllowedOperations(documentOperationClaim);
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

    private IEnumerable<OperationAuthorizationRequirement> GetAllowedOperations(Claim documentOperationClaim)
    {
        var result = new List<OperationAuthorizationRequirement>();
        var value = documentOperationClaim.Value;

        if (value.Contains('L'))
        {
            result.Add(Operations.List);
        }
        if (value.Contains('C'))
        {
            result.Add(Operations.Create);
        }
        if (value.Contains('R'))
        {
            result.Add(Operations.Read);
        }
        if (value.Contains('U'))
        {
            result.Add(Operations.Update);
        }
        if (value.Contains('D'))
        {
            result.Add(Operations.Delete);
        }

        return result;
    }
}
