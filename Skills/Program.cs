using System.DirectoryServices;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using Skills.Components;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;
using Skills.Services;

var builder = WebApplication.CreateBuilder(args);

var dataPath = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data/")).FullName;
builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/main.json"), optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/skills.json"), optional: false, reloadOnChange: true);


/* Microsoft Azure AD authentication */
var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ');

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    // options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Read", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Read)))
    .AddPolicy("Write", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Write)))
    .AddPolicy("Edit", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Edit)))
    .AddPolicy("Delete", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Delete)))
    .AddPolicy("Users", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Users)))
    .AddPolicy("Skills", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Skills)))
    .AddPolicy("Root", policy => policy.AddRequirements(new PermissionRequirement(PermissionPolicy.Root)));

builder.Services.Configure<OpenIdConnectOptions> (OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnSignedOutCallbackRedirect = context =>
    {
        context.HttpContext.Response.Redirect(context.Options.SignedOutRedirectUri);
        context.HandleResponse();
        return Task.CompletedTask;
    };

});
/* Microsoft Azure AD authentication */


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddMudServices();

/* Custom Services */
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddSingleton<ActiveDirectoryService>();
builder.Services.AddSingleton<ThemeManager>();
builder.Services.AddSingleton<SkillService>();
builder.Services.AddTransient<AuthenticationService>();
/* Custom Services */

/* Databases */
builder.Services.AddDbContextFactory<SkillsContext>();
/* Databases */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.UseAuthorization();
app.MapControllers();

var themeManager = app.Services.GetRequiredService<ThemeManager>();
var skillServices = app.Services.GetRequiredService<SkillService>();
await themeManager.InitAsync();
await skillServices.InitAsync();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await RunMigrationAsync<SkillsContext>(app);

// This service needs migrations to be performed before its InitAsync method is called because it may produce requests on the SkillsContext !
var adService = app.Services.GetRequiredService<ActiveDirectoryService>();
await adService.InitAsync();

await app.RunAsync();
return;


// Used to dynamically migrate project's databases when the app is launched (i.e. creates .db files in the respective folder)
async Task RunMigrationAsync<T>(IHost webApp) where T : DbContext
{
    var factory = webApp.Services.GetRequiredService<IDbContextFactory<T>>();
    var db = await factory.CreateDbContextAsync();
    await db.Database.MigrateAsync();
    await db.DisposeAsync();
}