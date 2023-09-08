using System.Security.Claims;

using AuthzDocuments.Authorization.Handlers;
using AuthzDocuments.Authorization.Transform;
using AuthzDocuments.Data;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthzDocuments;

// Bootstrapping the DB (sqlite):
// 1. Open the Package Manager Console
// 2. Add-Migration Initial
// 3. Update-Database


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            options.SignIn.RequireConfirmedAccount = false)
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddRazorPages();

        //builder.Services.AddServerSideBlazor();
        builder.Services.AddMarkdownEditor();

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Documents", policy => policy.RequireClaim("Documents"));
        });

        // Forgetting to add these handlers will make the authorization fail
        builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, AuthorAuthorizationHandler>();

        builder.Services.AddSingleton<IClaimsTransformation, CustomClaimTransformer>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCookiePolicy();
        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
        //app.MapBlazorHub();

        app.Run();
    }
}
