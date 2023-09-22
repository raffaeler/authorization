using AuthzForm.Authorization.Attributes;
using AuthzForm.Authorization.Handlers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;


[AuthorizeJuniors(5)]
public class Asset5Model : PageModel
{
    private readonly IAuthorizationService _authorizationService;

    public Asset5Model(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }
    public async Task OnGetAsync()
    {
        var auth = await _authorizationService.AuthorizeAsync(
            user: User,
            resource: null,
            requirement: new AuthorizeJuniorsAttribute(1));

        IsFirstYear = auth.Succeeded;
    }

    public bool IsFirstYear { get; set; }
}
