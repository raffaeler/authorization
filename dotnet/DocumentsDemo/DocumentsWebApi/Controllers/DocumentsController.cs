using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentsWebApi.Data;
using DocumentsWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DocumentsWebApi.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using DocumentsWebApi.Configurations;
using System.Text;

namespace DocumentsWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly ILogger<DocumentsController> _logger;
    private readonly DocumentsConfig _documentsConfig;
    private readonly DocumentsDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private string _fullPath;

    public DocumentsController(
        ILogger<DocumentsController> logger,
        IOptions<DocumentsConfig> documentsConfig,
        DocumentsDbContext context,
        IAuthorizationService authorizationService)
    {
        _logger = logger;
        _documentsConfig = documentsConfig.Value;
        _context = context;
        _authorizationService = authorizationService;

        _fullPath = Path.GetFullPath(_documentsConfig.Folder);
    }

    // GET: api/Documents
    [HttpGet]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Policy = Policies.DocsList)]
    public async Task<ActionResult<IEnumerable<FullDocument>>> GetDocuments()
    {
        _logger.LogInformation($"> {nameof(GetDocuments)} start");
        try
        {
            // Alternative to the Authorize attribute
            //var authResult = await _authorizationService.AuthorizeAsync(User, null, Policies.DocsList);
            //if (!authResult.Succeeded) return Unauthorized();

            List<FullDocument> result = new();
            foreach (var doc in _context.Documents.Include(x => x.Shares))
            {
                var effectivePermissions = await GetEffectivePermissions(doc);
                var fullDocument = new FullDocument(doc, string.Empty, effectivePermissions);
                result.Add(fullDocument);
            }

            return result;
        }
        finally
        {
            _logger.LogInformation($"> {nameof(GetDocuments)} end");
        }
    }

    // GET: api/Documents/5
    [HttpGet("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<FullDocument>> GetDocument(Guid id)
    {
        _logger.LogInformation($"> {nameof(GetDocument)} start");
        try
        {
            var document = await _context.Documents
            .Include(x => x.Shares)
            .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User, document, Operations.Read);
            if (!authResult.Succeeded)
            {
                return Unauthorized();
            }

            var markdown = await LoadMarkdown(document.Pathname);
            var effectivePermissions = await GetEffectivePermissions(document);
            FullDocument fullDocument = new(document, markdown, effectivePermissions);

            return fullDocument;
        }
        finally
        {
            _logger.LogInformation($"> {nameof(GetDocument)} end");
        }
    }

    // PUT: api/Documents/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> PutDocument(Guid id, FullDocument fullDocument)
    {
        _logger.LogInformation($"> {nameof(PutDocument)} start");
        try
        {
            if (id != fullDocument.Document.Id)
            {
                return BadRequest();
            }

            var current = await _context.Documents
                .Include(x => x.Shares)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (current == null)
            {
                return NotFound();
            }

            // authorization is applied on the document info saved on the DB
            // the document coming from the user may have been tampered
            var authResult = await _authorizationService.AuthorizeAsync(
                User, current, Operations.Update);
            if (!authResult.Succeeded)
            {
                return Unauthorized();
            }

            // modify the db with new, modified and deleted shares
            SynchronizeShares(current, fullDocument.Document);

            // we only copy the modifiable data in the current document
            current.Name = fullDocument.Document.Name;
            current.Description = fullDocument.Document.Description;
            _context.Entry(current).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                await SaveMarkdown(fullDocument.Document.Pathname, fullDocument.Markdown);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        finally
        {
            _logger.LogInformation($"> {nameof(PutDocument)} start");
        }
    }

    private void SynchronizeShares(Document dbDocument, Document newDocument)
    {
        var currentShares = dbDocument.Shares.ToList();

        foreach (var share in newDocument.Shares)
        {
            share.DocumentId = newDocument.Id;
            if (share.Id == Guid.Empty)
            {
                share.Id = Guid.NewGuid();
                _context.Shares.Add(share);
                //_context.Entry(share).State = EntityState.Added;
            }
            else
            {
                var currentShare = currentShares
                    .FirstOrDefault(s => s.Id == share.Id);
                if (currentShare == null)
                {
                    throw new Exception("The share Id is not associated with this document");
                }

                currentShare.Username = share.Username;
                currentShare.PermissionSet = share.PermissionSet;
                _context.Entry(currentShare).State = EntityState.Modified;

                currentShares.Remove(currentShare);
            }
        }

        // shares not within the collection that came from the client are deleted
        foreach (var currentShare in currentShares)
        {
            _context.Entry(currentShare).State = EntityState.Deleted;
        }
    }

    // POST: api/Documents
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Policy = Policies.DocsCreate)]
    public async Task<ActionResult<FullDocument>> PostDocument(FullDocument fullDocument)
    {
        _logger.LogInformation($"> {nameof(PostDocument)} start");
        try
        {
            _context.Documents.Add(fullDocument.Document);
            var filename = $"Doc_{fullDocument.Document.Id.ToString().ToLower()}";
            fullDocument.Document.Pathname = filename;

            var email = ((ClaimsIdentity?)User.Identity)
                ?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)
                ?.Value;
            if (email == null) return Unauthorized();

            fullDocument.Document.Author = email;

            await _context.SaveChangesAsync();
            await SaveMarkdown(filename, fullDocument.Markdown);

            return CreatedAtAction("GetDocument",
                new { id = fullDocument.Document.Id }, fullDocument);
        }
        finally
        {
            _logger.LogInformation($"> {nameof(PostDocument)} end");
        }
    }

    // DELETE: api/Documents/5
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeleteDocument(Guid id)
    {
        _logger.LogInformation($"> {nameof(DeleteDocument)} start");
        try
        {
            //var document = await _context.Documents.FindAsync(id);
            var document = await _context.Documents
            .Include(x => x.Shares)
            .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            var authResult = await _authorizationService.AuthorizeAsync(
                User, document, Operations.Delete);
            if (!authResult.Succeeded)
            {
                return Unauthorized();
            }

            _context.Documents.Remove(document);
            foreach (var share in document.Shares)
            {
                _context.Shares.Remove(share);
            }

            await _context.SaveChangesAsync();

            await DeleteMarkdown(document);

            return NoContent();
        }
        finally
        {
            _logger.LogInformation($"> {nameof(DeleteDocument)} end");
        }
    }

    private bool DocumentExists(Guid id)
    {
        return _context.Documents.Any(e => e.Id == id);
    }

    private async Task SaveMarkdown(string filename, string content)
    {
        var fullname = Path.Combine(_fullPath, filename);
        await System.IO.File.WriteAllTextAsync(fullname, content);
    }

    private async Task<string> LoadMarkdown(string filename)
    {
        var fullname = Path.Combine(_fullPath, filename);
        var content = await System.IO.File.ReadAllTextAsync(fullname);
        return content;
    }

    private Task DeleteMarkdown(Document document)
    {
        var filename = document.Pathname;
        var fullname = Path.Combine(_fullPath, filename);
        System.IO.File.Delete(fullname);
        return Task.CompletedTask;
    }


    private async Task<string> GetEffectivePermissions(Document document)
    {
        var create = await _authorizationService.AuthorizeAsync(User, document, Operations.Create);
        var read = await _authorizationService.AuthorizeAsync(User, document, Operations.Read);
        var update = await _authorizationService.AuthorizeAsync(User, document, Operations.Update);
        var delete = await _authorizationService.AuthorizeAsync(User, document, Operations.Delete);

        StringBuilder sb = new();
        if (create.Succeeded) sb.Append("C");
        if (read.Succeeded) sb.Append("R");
        if (update.Succeeded) sb.Append("U");
        if (delete.Succeeded) sb.Append("D");
        return sb.ToString();
    }

}
