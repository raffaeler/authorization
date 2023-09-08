using System.Collections.Generic;

using DocumentsWebApi.Models;

using Microsoft.EntityFrameworkCore;

namespace DocumentsWebApi.Data;

public class DocumentsDbContext : DbContext
{
	public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options)
        : base(options)
    {
	}

    public DbSet<Document> Documents { get; set; } = default!;
}
