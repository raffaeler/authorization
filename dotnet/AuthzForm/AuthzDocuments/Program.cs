using System.Security.Claims;

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

        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        builder.Services.AddAuthorization(options =>
        {
            //// frites = france + italy + spain
            //options.AddPolicy(MyPolicies.EconomicFrites, builder =>
            //    builder.RequireClaim(ClaimTypes.Country, "Italy", "France", "Spain"));

            //options.AddPolicy(MyPolicies.TechStaff, builder =>
            //{
            //    builder.Requirements.Add(new TechStaffRequirement());
            //});

            //options.AddPolicy(MyPolicies.SeniorTechStaff, builder =>
            //{
            //    // requirements are computed in AND
            //    builder.Requirements.Add(new TechStaffRequirement());
            //    builder.Requirements.Add(new SeniorRequirement(10));

            //    // this policy will need **both** technical and senior requirements
            //    // but the User may just be **either** a developer **or** an itpro
            //});
        });

        // Forgetting to add these handlers will make the authorization fail
        //builder.Services.AddSingleton<IAuthorizationHandler, DeveloperRequirementHandler>();
        //builder.Services.AddSingleton<IAuthorizationHandler, ItproRequirementHandler>();
        //builder.Services.AddSingleton<IAuthorizationHandler, SeniorRequirementHandler>();
        //builder.Services.AddSingleton<IAuthorizationHandler, SportRequirementHandler>();

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

        app.Run();
    }
}
