
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

using CommonAuth;

using DocumentsWebApi.Authorization;
using DocumentsWebApi.Authorization.Handlers;
using DocumentsWebApi.Authorization.Requirements;
using DocumentsWebApi.Configurations;
using DocumentsWebApi.Data;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DocumentsWebApi;

// Bootstrapping the DB (sqlite):
// 1. Open the Package Manager Console
// 2. Add-Migration Initial
// 3. Update-Database

public class Program
{
    public static void Main(string[] args)
    {
        var corsPolicy = "MyCorsPolicy";
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<DocumentsDbContext>(options =>
            options.UseSqlite(connectionString));


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsPolicy, policy =>
            {
                policy
                    //.AllowAnyOrigin()
                    .WithOrigins("https://localhost:3443", "https://local:3443",
                                "https://spa.iamraf.net:3443", "http://spa.iamraf.net:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var documentsConfigSection = builder.Configuration.GetSection("DocumentsConfig");
        builder.Services.Configure<DocumentsConfig>(documentsConfigSection);

        var authServerSection = builder.Configuration.GetSection("AuthServer");
        builder.Services.Configure<AuthServerConfiguration>(authServerSection);
        var authServerConfig = authServerSection.Get<AuthServerConfiguration>();
        if (authServerConfig == null)
            throw new Exception($"Missing AuthServer configuration");

        // === Start authorization config ===
        // HttpContextAccessor is needed to access HttpContext from the requirement handler
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthorization(c =>
        {
            c.AddPolicy("mfa", bld => bld.Requirements.Add(new OtpRequirement("mfa")));
            c.AddPolicy("hwk", bld => bld.Requirements.Add(new OtpRequirement("hwk")));

            //c.AddPolicy(Policies.DeviceAdmin, bld => bld.Requirements.Add(new )
        });

        builder.Services.AddScoped<IAuthorizationHandler, OtpRequirementHandler>();
        // === End authorization config ===

        // === start authentication config ===
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddKeycloak(authServerConfig, new OpenIdConnectEvents()
        {
            OnRedirectToIdentityProvider = ctx =>
            {
                // specify additional parameters here
                if (ctx.HttpContext.Items.TryGetValue("acr", out object? acrValue))
                {
                    var acr = acrValue as string ?? string.Empty;  // "mfa"
                    ctx.ProtocolMessage.SetParameter("acr_values", acr);
                }

                if (ctx.HttpContext.Items.TryGetValue(
                    AuthorizationHelper.ScopesKey,
                    out object? scopesValue))
                {
                    var scopes = scopesValue as string[] ?? Array.Empty<string>();
                    ctx.ProtocolMessage.Scope += " " + string.Join(" ", scopes);
                }

                return Task.CompletedTask;
            },

            OnRedirectToIdentityProviderForSignOut = ctx => Dbg("OnRedirectToIdentityProviderForSignOut"),

            OnAccessDenied = ctx => Dbg($"OIDC: OnAccessDenied"),
            OnAuthenticationFailed = ctx => Dbg($"OIDC: OnAuthenticationFailed"),
            OnAuthorizationCodeReceived = ctx => Dbg($"OIDC: OnAuthorizationCodeReceived"),
            OnMessageReceived = ctx => Dbg($"OIDC: OnMessageReceived"),
            OnRemoteSignOut = ctx => Dbg($"OIDC: OnRemoteSignOut"),
            OnSignedOutCallbackRedirect = ctx => Dbg($"OIDC: OnSignedOutCallbackRedirect"),
            OnTicketReceived = ctx => Dbg($"OIDC: OnTicketReceived"),
            OnTokenResponseReceived = ctx => Dbg($"OIDC: OnTokenResponseReceived"),
            OnTokenValidated = ctx => Dbg($"OIDC: OnTokenValidated"),
            OnUserInformationReceived = ctx =>
            {
                //var tokens = ctx.Properties.GetTokens().ToList();
                //var access_token = tokens.FirstOrDefault(t => t.Name == "access_token");
                //var id_token = tokens.FirstOrDefault(t => t.Name == "id_token");
                //if (access_token != null && id_token != null)
                //{
                //    tokens.Remove(id_token);
                //    ctx.Properties.StoreTokens(tokens);
                //}

                return Dbg($"OIDC: OnUserInformationReceived");
            },

            OnRemoteFailure = ctx =>
            {
                // apparently this OIDC client does not handle 'error_uri'

                // ctx.Failure contains the error message
                // we should redirect to a page that shows the message

                //ctx.Response.Redirect("/");
                //ctx.HandleResponse();
                return Task.CompletedTask;
            }

        })
        .AddJwtBearer(options =>
        {
            var jwtHandler = (options.SecurityTokenValidators.FirstOrDefault() as JwtSecurityTokenHandler);
            if (jwtHandler != null)
            {
                // false: use the short names. For example: "acr"
                // true: use the long uri type names
                // names are used into the requirements to search the claims
                jwtHandler.MapInboundClaims = false;
            }

            options.MetadataAddress = authServerConfig.MetadataAddress;
            options.RequireHttpsMetadata = false;
            options.Audience = "DocsBackend";
            options.Authority = authServerConfig.Authority;
            options.Events = new JwtBearerEvents()
            {
                OnChallenge = x => Dbg($"JWT: Challenge "),
                OnMessageReceived = x => Dbg($"JWT: OnMessageReceived "),
                OnAuthenticationFailed = x => Dbg($"JWT: {x.Exception.ToString()}"),
                OnTokenValidated = x => Dbg($"JWT: Token has been validated: {x.Result}"),
                OnForbidden = x => Dbg($"JWT: Forbidden"),
            };

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                ClockSkew = TimeSpan.FromMinutes(5),
            };

            // this should never be disabled
            // it is done in the demo because we use:
            // - https://host.docker.internal:8443 from this project
            // - https://kc.iamraf.net:8443 when demo-ed by raf using a custom DNS
            // - https://local:8443 from the SPA client project
            options.TokenValidationParameters.ValidateIssuer = false;
            //options.TokenValidationParameters.ValidateAudience = false;   // use this only to test audience issues
        });
        // === end authentication config ===


        // === start authorization config ===
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Documents", policy => policy.RequireClaim("Documents"));
            options.AddPolicy(Policies.DocsList,
                policy => policy.AddRequirements(Operations.List));

            options.AddPolicy(Policies.DocsCreate,
                policy => policy.AddRequirements(Operations.Create));

            options.AddPolicy(Policies.DocsRead,
                policy => policy.AddRequirements(Operations.Read));

            options.AddPolicy(Policies.DocsUpdate,
                policy => policy.AddRequirements(Operations.Update));

            options.AddPolicy(Policies.DocsDelete,
                policy => policy.AddRequirements(Operations.Delete));
        });

        // Forgetting to add these handlers will make the authorization fail
        builder.Services.AddSingleton<IAuthorizationHandler, DocumentOperationAuthorizationHandler>();
        //builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, AuthorAuthorizationHandler>();

        // === end authorization config ===


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(corsPolicy);
        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }

    private static Task Dbg(string caller)
    {
        Debug.WriteLine(caller);
        return Task.CompletedTask;
    }
}
