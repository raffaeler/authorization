using System.Security.Claims;

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

    public static IEnumerable<OperationAuthorizationRequirement> GetAllowedOperations(string crud)
    {
        var result = new List<OperationAuthorizationRequirement>();

        if (crud.Contains('L'))
        {
            result.Add(Operations.List);
        }
        if (crud.Contains('C'))
        {
            result.Add(Operations.Create);
        }
        if (crud.Contains('R'))
        {
            result.Add(Operations.Read);
        }
        if (crud.Contains('U'))
        {
            result.Add(Operations.Update);
        }
        if (crud.Contains('D'))
        {
            result.Add(Operations.Delete);
        }

        return result;
    }

    public static string GetShortString(OperationAuthorizationRequirement operation)
    {
        var result = operation.Name switch
        {
            "List" => "L",
            "Create" => "C",
            "Read" => "R",
            "Update" => "U",
            "Delete" => "D",
            _ => throw new Exception($"Unknown operation {operation.Name}"),
        };

        return result;
    }

}
