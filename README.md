# Authorization in .NET 8 
This is the repository for my session about authorization in .NET 8.

It contains three projects for two different demos:

- `dotnet/AuthzForm` is an ASP.NET 8 (RC1) showing the basics of .NET Authorization as well as how to use `IAuthorizationRequirementData`, which is new on .NET 8
- `dotnet/DocumentsDemo` is a solution composed by:
  - `DocumentsWebApi` ASP.NET 8 (RC1) Web API. This is the backend for the application
  - `CommonAuth` is a library with some tools for the OIDC authentication on `Keycloak`
- `spa/docmanager` is the React front-end application for the previous `DocumentsWebApi` project

The `AuthzForm` project is straightforward and can be run from Visual Studio.

The other demo requires `Keycloak` container. The instructions to configure a running instance of `Keycloak` can be found in the `DocumentsWebApi` folder in the `docs-demo-config.md` document.

### Important note

You can easily adapt the authorization code to any other identity providers. Authentication and authorization are well decoupled in ASP.NET. Anyway, the claim names are very important to provide the correct behavior in the application. The code provides the relevant information. If you can't figure out this, please open an issue and I will follow-up.



If you have any questions about this code and on the talk, please open an issue in this repository.



















