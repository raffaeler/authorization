using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AuthzDocuments.Data;
using AuthzDocuments.Models;
using System.Security.Claims;

namespace AuthzDocuments.Pages;

public class CreateModel : PageModel
{
    private readonly AuthzDocuments.Data.ApplicationDbContext _context;

    public CreateModel(AuthzDocuments.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public Document Document { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || User.Identity == null)
        {
            return Page();
        }

        var email = ((ClaimsIdentity)User.Identity)
            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        if (email == null) return Page();

        Document.Author = email;

        _context.Document.Add(Document);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
