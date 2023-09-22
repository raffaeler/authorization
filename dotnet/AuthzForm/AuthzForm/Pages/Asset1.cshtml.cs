using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;

[Authorize]
public class Asset1Model : PageModel
{
    public void OnGet()
    {
    }
}
