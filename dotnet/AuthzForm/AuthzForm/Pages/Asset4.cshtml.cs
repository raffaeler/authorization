using AuthzForm.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;

[Authorize(Policy = MyPolicies.SeniorTechStaff)]
public class Asset4Model : PageModel
{
    public void OnGet()
    {
    }
}
