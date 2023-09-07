using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;

namespace AuthzForm.Authorization.Transform;

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
                // real
                //identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, new DateTime(1967, 02, 26).ToString(CultureInfo.InvariantCulture)));

                // 17
                identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, new DateTime(1999, 02, 26).ToString(CultureInfo.InvariantCulture)));

                // 19
                //identity.AddClaim(new Claim(ClaimTypes.DateOfBirth, new DateTime(1997, 02, 26).ToString(CultureInfo.InvariantCulture)));


                identity.AddClaim(new Claim(identity.RoleClaimType, "Administrators"));
                //identity.AddClaim(new Claim(identity.RoleClaimType, "Users"));

                identity.AddClaim(new Claim(AuthorizationHelpers.DepartmentClaimType, "Developers"));
                identity.AddClaim(new Claim(AuthorizationHelpers.YearsOfExperienceClaimType,
                    (DateTime.Now.Year - 1987).ToString(), ClaimValueTypes.Integer));


                //identity.AddClaim(new Claim(ClaimTypes.Country, "Italy"));
                identity.AddClaim(new Claim(ClaimTypes.Country, "Switzerland"));

                // Manually adding the LinkedIn Skills
                identity.AddClaim(new Claim("Skills", ".NET"));
                identity.AddClaim(new Claim("Skills", "C#"));
                identity.AddClaim(new Claim("Skills", "ASP.NET"));
                identity.AddClaim(new Claim("Skills", "Software Architecture"));
                identity.AddClaim(new Claim("Skills", "Digital Hardware"));
                identity.AddClaim(new Claim("Skills", "IoT"));

                AuthorInfo authorInfo = new(
                    new[]
                    {
                        new Book("Elettronica Applicata", "xyz"),
                        new Book("C# Programming", "xyz"),
                    }, new[] { "Run", "Swim" });

                var json = JsonSerializer.Serialize(authorInfo);
                identity.AddClaim(new Claim(nameof(AuthorInfo), json, "JSON"));
                break;
        }
    }

}
