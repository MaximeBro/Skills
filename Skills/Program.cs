using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Skills.Components;
using Skills.Services;

var builder = WebApplication.CreateBuilder(args);

var dataPath = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../data/")).FullName;
builder.Configuration.AddJsonFile(Path.Combine(dataPath, "config/main.json"), optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

/* Custom Services */
builder.Services.AddSingleton<ThemeService>();
/* Custom Services */

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
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

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