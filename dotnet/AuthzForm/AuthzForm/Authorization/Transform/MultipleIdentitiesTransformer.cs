using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace AuthzForm.Authorization.Transform;

public class MultipleIdentitiesTransformer : IClaimsTransformation
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly CustomClaimTransformer _customClaimTransformer;

    public MultipleIdentitiesTransformer(
        IHttpContextAccessor httpContextAccessor,
        CustomClaimTransformer customClaimTransformer)
	{
        _httpContextAccessor = httpContextAccessor;
        _customClaimTransformer = customClaimTransformer;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var newPrincipal = await _customClaimTransformer.TransformAsync(principal);

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return newPrincipal;
        var headers = httpContext.Request.Headers;

        ClaimsIdentity? identity;
        if (!headers.TryGetValue("X-WhateverYouWant", out var value) ||
            value[0] == StringValues.Empty ||
            (identity = CreateIdentityFromJwtToken(value[0])) == null)
        {
            identity = CreateFakeIdentity();
        }

        if(identity != null) newPrincipal.AddIdentity(identity);
        return newPrincipal;
    }

    private ClaimsIdentity? CreateIdentityFromJwtToken(string? token)
    {
        if (token == null) return null;
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // configure the validator and validate the token
        //TokenValidationParameters validator = new();
        //validator...

        ClaimsIdentity identity = new(jwt.Claims,
            JwtBearerDefaults.AuthenticationScheme);

        return identity;
    }


    private ClaimsIdentity? CreateFakeIdentity()
    {
        return new ClaimsIdentity(new Claim[]
        {
            //new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "John Doe"),
            new Claim(ClaimTypes.Email, "john.doe@email.com"),
            new Claim(ClaimTypes.Country, "New York"),
        });
    }



}
