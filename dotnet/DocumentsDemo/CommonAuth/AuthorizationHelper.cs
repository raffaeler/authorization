using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CommonAuth;

public static class AuthorizationHelper
{
    public const string ScopesKey = "scopes";
    public static void AddScopes(HttpContext context, params string[] scopes)
    {
        context.Items.TryGetValue(ScopesKey, out object? itemValue);
        string[] scopeValues = itemValue as string[] ?? Array.Empty<string>();

        var mergedScopes = new List<string>(scopes.Length + scopeValues.Length);
        mergedScopes.AddRange(scopeValues);
        mergedScopes.AddRange(scopes);
        context.Items[ScopesKey] = mergedScopes.Distinct().ToArray();
    }

    public static bool TryGetScope(
        this ClaimsPrincipal principal, out string[] values)
    {
        string claimName = "scope";
        values = principal.Claims
            .Where(c => c.Type == claimName && c.ValueType == "string")
            .Select(c => c.Value.Split(' ',
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .SelectMany(c => c)
            .ToArray();

        if (values.Length == 0) return false;
        return true;
    }

    public static bool TryGetClaimJsonValue<T>(
        this ClaimsPrincipal principal, string claimName, out T? value)
    {
        value = default(T);

        var claim = principal.Claims
            .FirstOrDefault(c => c.Type == claimName);

        if (claim == null || claim.ValueType != "JSON")
            return false;

        var obj = JsonSerializer.Deserialize<T>(claim.Value);
        if (obj == null)
            return false;

        value = obj;
        return true;
    }

    public static bool TryGetStringValue(
        this ClaimsPrincipal principal, string claimName, out string value)
    {
        value = string.Empty;

        var claim = principal.Claims
            .FirstOrDefault(c => c.Type == claimName);

        if (claim == null || claim.ValueType != ClaimValueTypes.String)
            return false;

        value = claim.Value;
        return true;
    }
}

