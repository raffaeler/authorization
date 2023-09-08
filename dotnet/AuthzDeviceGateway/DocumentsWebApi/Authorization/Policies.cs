namespace DocumentsWebApi.Authorization;

public class Policies
{
    public const string OtpMfa = "mfa";
    public const string OtpHwk = "hwk";

    public const string DocsCreate = "docs-create";
    public const string DocsRead = "docs-read";
    public const string DocsUpdate = "docs-update";
    public const string DocsDelete = "docs-delete";

}
