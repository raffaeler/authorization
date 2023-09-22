using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;

[Authorize(Roles="Administrators,PowerUsers")]
public class Asset2Model : PageModel
{
    public void OnGet()
    {
    }
}
