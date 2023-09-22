namespace DocumentsWebApi.Models;

public record FullDocument(
    Document Document,
    string Markdown,
    string EffectivePermissions);
