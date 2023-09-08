namespace AuthzDocuments.Models;

public record Document(Guid Id,
    string Name,
    string Description,
    string Pathname,
    string Author);
