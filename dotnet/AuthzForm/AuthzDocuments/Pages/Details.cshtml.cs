using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AuthzDocuments.Data;
using AuthzDocuments.Models;

namespace AuthzDocuments.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly AuthzDocuments.Data.ApplicationDbContext _context;

        public DetailsModel(AuthzDocuments.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Document Document { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Document.FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                return NotFound();
            }
            else
            {
                Document = document;
                await LoadContent();
            }
            return Page();
        }

        public string Markdown { get; set; } = "# Empty";

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
}
