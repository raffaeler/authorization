namespace DocumentsWebApi.Models;

public class Share
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PermissionSet { get; set; } = string.Empty;
}
