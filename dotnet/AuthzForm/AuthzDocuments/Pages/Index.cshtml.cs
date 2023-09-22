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
    public class IndexModel : PageModel
    {
        private readonly AuthzDocuments.Data.ApplicationDbContext _context;

        public IndexModel(AuthzDocuments.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Document> Document { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Document = await _context.Document.ToListAsync();
        }
    }
}
