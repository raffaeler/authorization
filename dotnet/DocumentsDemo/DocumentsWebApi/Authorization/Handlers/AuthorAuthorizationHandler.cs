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

        var username = context.User.Identity.Name;
        var document = resource;

        var email = ((ClaimsIdentity)context.User.Identity)
                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
        if (email == null)
        {
            _logger.LogInformation($"Requirement not met (invalid email) - User:{username} Document:{document.Id}");
            return Task.CompletedTask;
        }

        if (string.Compare(document.Author, email, true) == 0)
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
