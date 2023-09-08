using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuthzDocuments.Data;
using AuthzDocuments.Models;

namespace AuthzDocuments.Pages;

public class EditModel : PageModel
{
    private readonly AuthzDocuments.Data.ApplicationDbContext _context;

    public EditModel(AuthzDocuments.Data.ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Document Document { get; set; } = default!;

    [BindProperty]
    public string Markdown { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var document =  await _context.Document.FirstOrDefaultAsync(m => m.Id == id);
        if (document == null)
        {
            return NotFound();
        }
        Document = document;
        await LoadContent();
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Document).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!DocumentExists(Document.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool DocumentExists(Guid id)
    {
        return _context.Document.Any(e => e.Id == id);
    }

    private async Task LoadContent()
    {
        try
        {
            var path = Path.Combine("_mdfiles", Document.Pathname);
            Markdown = await System.IO.File.ReadAllTextAsync(path);
        }
        catch (Exception)
        {
            Markdown = string.Empty;
        }
    }

}
