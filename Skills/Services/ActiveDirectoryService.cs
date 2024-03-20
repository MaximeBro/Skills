using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Extensions;

namespace Skills.Services;


[SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
public class ActiveDirectoryService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public readonly List<UserPrincipal> DirectoryEntries = new();

    public async Task InitAsync()
    {
        DirectoryEntries.AddRange(GetActiveDirectoryUsers());
        await CheckUsersAsync();
    }
    
    private List<UserPrincipal> GetActiveDirectoryUsers()
    {
        var context = new PrincipalContext(ContextType.Domain, configuration["ip"]);
        var searcher = new PrincipalSearcher(new UserPrincipal(context));
        var results = searcher.FindAll().Cast<UserPrincipal>().ToList();
        var collaborators = results.Where(x => !string.IsNullOrWhiteSpace(x.EmailAddress) && x.EmailAddress.EndsWith("@sasp.fr")).ToList();

        return collaborators;
    }

    private async Task CheckUsersAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var savedEmails = await db.Users.AsNoTracking().Select(x => x.Email).ToListAsync();
        var toAdd = DirectoryEntries.Where(x => !savedEmails.Contains(x.EmailAddress, StringComparer.OrdinalIgnoreCase)).ToList();

        await db.Users.AddRangeAsync(toAdd.ToUserModel());
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }
}