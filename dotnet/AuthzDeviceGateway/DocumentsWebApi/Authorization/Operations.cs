using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace DocumentsWebApi.Authorization;

public static class Operations
{
    public static OperationAuthorizationRequirement List =
        new OperationAuthorizationRequirement { Name = "List" };

    public static OperationAuthorizationRequirement Create =
        new OperationAuthorizationRequirement { Name = "Create" };

    public static OperationAuthorizationRequirement Read =
        new OperationAuthorizationRequirement { Name = "Read" };

    public static OperationAuthorizationRequirement Update =
        new OperationAuthorizationRequirement { Name = "Update" };

    public static OperationAuthorizationRequirement Delete =
        new OperationAuthorizationRequirement { Name = "Delete" };
}
