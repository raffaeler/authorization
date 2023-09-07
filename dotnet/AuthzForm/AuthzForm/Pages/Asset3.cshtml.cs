using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthzForm.Pages;

// frites = france + italy + spain economic group
[Authorize(Policy= "economic_frites")]
public class Asset3Model : PageModel
{
    public void OnGet()
    {
    }
}
