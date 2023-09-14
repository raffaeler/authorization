using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DocumentsWebApi.Models;

namespace DocumentsWebApi.Authorization.Handlers;

public class AuthorAuthorizationHandler :
    AuthorizationHandler<OperationAuthorizationRequirement, Document>
{
    private readonly ILogger<AuthorAuthorizationHandler> _logger;

    public AuthorAuthorizationHandler(ILogger<AuthorAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Document resource)
    {
        //context.Succeed(requirement);
        //return Task.CompletedTask;

        if (context.User.Identity == null ||
            resource == null) return Task.CompletedTask;

        var user = context.User;
        var document = resource;

        var email = ((ClaimsIdentity)user.Identity)
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
        if (email == null)
        {
            _logger.LogInformation($"Requirement not met (invalid email) - User:{user} Document:{document.Id}");
            return Task.CompletedTask;
        }

        if (string.Compare(document.Author, email, true) == 0)
        {
            _logger.LogInformation($"Requirement succeeded - User:{user} Document:{document.Id}");

            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation($"Requirement not met - User:{user} Document:{document.Id}");
        }

        return Task.CompletedTask;
    }
}
