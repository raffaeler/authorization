namespace AuthzDocuments.Models;

public record Document
{
    public static Document Empty = new Document();

    public Document()
        : this(Guid.Empty, string.Empty, string.Empty, string.Empty)
    {
    }

    public Document(Guid id, string name, string description, string pathname)
    {
        Id = id;
        Name = name;
        Description = description;
        Pathname = pathname;
        Author = string.Empty;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Pathname { get; set; }

    public string Author { get; set; }
}
