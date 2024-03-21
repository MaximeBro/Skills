using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.Enums;

namespace Skills.Services;


[SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
public class ActiveDirectoryService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public readonly List<UserPrincipal> DirectoryEntries = new();

    public async Task InitAsync()
    {
        DirectoryEntries.AddRange(GetActiveDirectoryUsers());
        await CheckUsersAsync();
        await SetRootsAsync();
    }
    
    /// <summary>
    /// Retrieves all the users in the local AD.
    /// </summary>
    /// <returns>All the collaborators registered in the local AD.</returns>
    private List<UserPrincipal> GetActiveDirectoryUsers()
    {
        var context = new PrincipalContext(ContextType.Domain, configuration["ip"]);
        var searcher = new PrincipalSearcher(new UserPrincipal(context));
        var results = searcher.FindAll().Cast<UserPrincipal>().ToList();
        var collaborators = results.Where(x => !string.IsNullOrWhiteSpace(x.EmailAddress) && x.EmailAddress.EndsWith("@sasp.fr")).ToList();

        return collaborators;
    }

    /// <summary>
    /// Retrieves all the users in the DirectoryEntries and inserts missing ones in the app db.
    /// </summary>
    private async Task CheckUsersAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var savedEmails = await db.Users.AsNoTracking().Select(x => x.Email).ToListAsync();
        var toAdd = DirectoryEntries.Where(x => !savedEmails.Contains(x.EmailAddress, StringComparer.OrdinalIgnoreCase)).ToList();

        await db.Users.AddRangeAsync(toAdd.ToUserModel());
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }

    /// <summary>
    /// Retrieves the specified email addresses at "default-roots" in the skills.json config and set PermissionPolicy.Root to their db stored account.  
    /// </summary>
    private async Task SetRootsAsync()
    {
        var db = await factory.CreateDbContextAsync();
        
        var data = configuration["default-roots"] ?? string.Empty;
        var emails = data.Split(",");
        foreach (var email in emails)
        {
            var user = await db.Users.AsTracking().FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                user.Policy = PermissionPolicy.Root;
            }
        }
        
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }
}