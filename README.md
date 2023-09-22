# Authorization in .NET 8 
This is the repository for my session about authorization in .NET 8.

The `dotnet` folder contains a solution with an ASP.NET 8 application with the following characteristics:

- Based on .NET 8 Preview 7

- The Identity Provider is `Keycloak` (which I use locally in container) configured as follows:

  - The realm is called `Demo`

  - The Client is called `AspNetMvc`

    - The `OpenIdConnect` associated flow is the `authorization_code` grant 

  - The file `appsettings.json` contains the client secret.

    *This is not important as I run `Keycloak` locally in a container demo which is not reachable from a production environment or the public Internet. However, any secret should always be configured in the environment variables following the best-practices.*

  - The `CommonAuth` library contains the `KeycloakServiceExtensions` with most of the `OpenIdConnect` and `Cookie` providers configuration.

  - In order to make this demo work, you have to properly configure this container. I use a local `DNS server` and `DHCP server` so that I can properly make `oAuth2` redirects.

  - I also use `self-signed` certificates for both `TLS` and `Client authentication` (`mTLS`) which are deployed on my own machine and inside the `Keycloak` container.

- The ASP.NET application uses the `Razor pages` as well as `WebApi` controllers

- The `Debug` page shows the three `openid` tokens.

### Important note

You can easily adapt the authorization code to any other identity providers. Authentication and authorization are well decoupled in ASP.NET. Anyway, the claim names are very important to provide the correct behavior in the application. The code provides the relevant information. If you can't figure out this, please open an issue and I will follow-up.

### Device Gateway Application

The use-case for this demo application is using and administering the gateway for IoT devices. You can imagine there are IoT devices in the field which can be read and/or written. The gateway is responsible for letting the users interact with those devices. The application therefore enforce both authentication and authorization:

- Authentication is used to identify the user. Since the authentication is federated, this process is delegated to an external Identity Provider, in this case a `Keycloak` container, using the `authorization_code` grant. The `IP` will return three tokens:

  - `id token` is the token used to enforce the authorization in this application
  - `access token` is the `Bearer` `JWT` token used from the application to interact with any other client of the same realm, provided that the `audience` claim is appropriately set
  - `refresh token` is the token used from the application to renew/refresh the three tokens as needed. This is convenient to avoid the user to be forced to logon every time the tokens expire.

- Authorization is used to understand what the users **can** or **cannot** do in the application. The authorization framework was re-designed in ASP.NET Core to **decouple** the authorization logic from the business logic. This is an important step to understand which avoids a lot of headaches when dealing with a real-life scenario.

  In ASP.NET Core 8, the new interface `IAuthorizationRequirementData` simplify the amount of code required to setting up the authorization logic.

  My talk on the Authorization focuses specifically on this part but of course provides a basic understanding of the steps needed to request, manage and transform the token created by Identity Provider.



If you have any questions about this code and on the talk, please open an issue in this repository.



















