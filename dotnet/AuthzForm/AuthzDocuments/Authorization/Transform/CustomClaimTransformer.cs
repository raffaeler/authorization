using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;

namespace AuthzDocuments.Authorization.Transform;

public record AuthorInfo(IEnumerable<Book> Books, IEnumerable<string> Sports);
public record Book(string Name, string Isbn);

public class CustomClaimTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        InjectClaims(principal.Identity as ClaimsIdentity);
        return Task.FromResult(principal);
    }

    private void InjectClaims(ClaimsIdentity? identity)
    {
        if (identity == null || !identity.IsAuthenticated)
        {
            return;
        }

        switch (identity.Name)
        {
            case "raffaeler@vevy.com":
                identity.AddClaim(new Claim("Documents", "L"));
                break;

            case "raf@vevy.com":
                identity.AddClaim(new Claim("Documents", "LCR"));
                break;

            case "admin@vevy.com":
                identity.AddClaim(new Claim("Documents", "LCRUD"));
                break;

        }
    }

}
