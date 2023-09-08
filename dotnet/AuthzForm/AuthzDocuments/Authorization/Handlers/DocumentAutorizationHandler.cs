using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuthzDocuments.Models;

namespace AuthzDocuments.Authorization.Handlers;

public class DocumentAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Document>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        Document resource)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;

        var documentOperationClaim = context.User.Claims
            .FirstOrDefault(c => c.Type == "Documents");

        if (documentOperationClaim != null)
        {
            var user = context.User;
            var document = resource;

            var operations = GetAllowedOperations(documentOperationClaim);
            if (operations.Contains(requirement))
            {
                context.Succeed(requirement);
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
