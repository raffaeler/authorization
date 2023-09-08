using AuthzForm.Authorization;
using AuthzForm.Authorization.Requirements;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;

[Authorize(Policy = MyPolicies.SeniorTechStaff)]
public class Asset4Model : PageModel
{
    private readonly IAuthorizationService _authorizationService;

    public Asset4Model(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnGetAsync()
    {
        var requirement = new SportRequirement();

        var auth = await _authorizationService.AuthorizeAsync(
            user:User,
            resource: null,
            requirement: requirement);

        IsSportActive = auth.Succeeded;
    }

    public bool IsSportActive { get; set; }
}
