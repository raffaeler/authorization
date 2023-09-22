namespace DocumentsWebApi.Configurations;

public class DocumentsConfig
{
    /// <summary>
    /// The folder where the files are stored
    /// It can be relative or absolute
    /// It can use the forward slash so that it can run on Linux and Windows
    /// </summary>
    public string Folder { get; set; } = string.Empty;
}
