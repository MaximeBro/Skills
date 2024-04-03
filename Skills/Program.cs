using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Skills.Components;
using Skills.Databases;
using Skills.Extensions;
using Skills.Services;

var builder = WebApplication.CreateBuilder(args);
var dataPath = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "/data/")).FullName;

#if DEBUG
dataPath = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data/")).FullName;
#endif

builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/main.json"), optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/skills.json"), optional: false, reloadOnChange: true);

builder.Services.AddHttpContextAccessor(); // Required for authentication, take a look at ServiceExtensions.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/";
        options.Cookie.Name = "Skills";
    });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

/* Custom Services */
builder.Services.AddTransient<ADAuthenticationService>();
builder.Services.AddSingleton<UserTokenHoldingService>();
builder.Services.AddSingleton<ActiveDirectoryService>();
builder.Services.AddSingleton<ThemeManager>();
builder.Services.AddSingleton<SkillService>();
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

app.UseAuthentication();
app.UseAuthorization();
app.UseLogin();
app.UseLogout();
app.UseFileUpload();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var themeManager = app.Services.GetRequiredService<ThemeManager>();
var skillServices = app.Services.GetRequiredService<SkillService>();
await themeManager.InitAsync();
await skillServices.InitAsync();

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