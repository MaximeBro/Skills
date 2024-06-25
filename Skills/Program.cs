using System.Net;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Skills.Components;
using Skills.Databases;
using Skills.Extensions;
using Skills.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5000", "http://localhost:5001");


var dataPath = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data")).FullName;

// var dataPath = new DirectoryInfo(Path.Combine(builder.Environment.WebRootPath, "../data")).FullName;
Console.WriteLine($"DATA PATH ROUTING -> {dataPath}");
Console.WriteLine($"ENVIRONMENT USERNAME -> {Environment.UserName}");
Console.WriteLine($"ENVIRONMENT DOMAIN -> {Environment.UserDomainName}");
builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/main.json"), optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/skills.json"), optional: false, reloadOnChange: true);

builder.Services.AddHttpContextAccessor(); // Required for authentication, take a look at ServiceExtensions.cs
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/connexion";
        options.AccessDeniedPath = "/";
        options.Cookie.Name = "Skills";
    });

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options => options.MaximumReceiveMessageSize = 1 * 1024 * 1024); // 1 MB
builder.Services.AddMudServices();


/* Custom Services */
builder.Services.AddTransient<ADAuthenticationService>();
builder.Services.AddSingleton<WordExportService>();
builder.Services.AddSingleton<UserTokenHoldingService>();
builder.Services.AddSingleton<ActiveDirectoryService>();
builder.Services.AddSingleton<ThemeManager>();
builder.Services.AddSingleton<LocalizationManager>();
builder.Services.AddSingleton<SkillService>();
builder.Services.AddSingleton<RealTimeUpdateService>();
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
var localizationManager = app.Services.GetRequiredService<LocalizationManager>();
await themeManager.InitAsync();
await localizationManager.InitAsync();

await RunMigrationAsync<SkillsContext>(app);

// These services need migrations to be performed before their InitAsync method is called because it may produce requests on the SkillsContext !
var adService = app.Services.GetRequiredService<ActiveDirectoryService>();
var skillService = app.Services.GetRequiredService<SkillService>();
await adService.InitAsync();
await skillService.InitAsync();

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
