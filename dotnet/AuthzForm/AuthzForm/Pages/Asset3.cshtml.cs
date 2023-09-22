using AuthzForm.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;

//// frites = france + italy + spain economic group
//[Authorize(Policy = "economic_frites")]
public class Asset3Model : PageModel
{
    private readonly IAuthorizationService _authorizationService;

    public Asset3Model(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public Task OnGetAsync()
    {
        return LoadData();
    }

    private async Task LoadData()
    {
        var auth = await _authorizationService.AuthorizeAsync(
            User, MyPolicies.EconomicFrites);
        if (!auth.Succeeded)
        {
            CanRead = false;
            ReservedData = string.Empty;
            return;
        }

        CanRead = true;
        ReservedData = "This data is protected";
    }

    public bool CanRead { get; set; }
    public string ReservedData { get; set; } = string.Empty;
}
