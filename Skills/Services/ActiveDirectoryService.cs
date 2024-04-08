using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models;
using Skills.Models.Enums;

namespace Skills.Services;


[SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
public class ActiveDirectoryService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public readonly List<UserModel> DirectoryEntries = new();

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
    private List<UserModel> GetActiveDirectoryUsers()
    {
        try
        {
            var collaborators = new List<UserModel>();
            var entry = new DirectoryEntry("LDAP://sasp.local", configuration.GetSection("DirectoryServices")["login"], configuration.GetSection("DirectoryServices")["password"]);
            var searcher = new DirectorySearcher(entry, "(objectClass=user)");
            var users = searcher.FindAll().Cast<SearchResult>().ToList();
            foreach (var user in users.Where(x => x.Properties.Contains("mail")).ToList())
            {
                var directoryEntry = user.GetDirectoryEntry();
                var flags = Convert.ToInt32(directoryEntry.Properties["userAccountControl"][0]?.ToString() ?? string.Empty);
                
                var model = new UserModel
                {
                    Email = directoryEntry.Properties["mail"].Value?.ToString() ?? string.Empty,
                    IsDisabled = Convert.ToBoolean(flags & 0x00000002) // 0x00000002 == ACCOUNT_DISABLED
                };
                model.Username = model.Email.Replace("@sasp.fr", String.Empty);
                model.Name = $"{model.Email.Replace("@sasp.fr", string.Empty).Split(".")[0].FirstCharToUpper()} {model.Email.Replace("@sasp.fr", string.Empty).Split(".")[1].FirstCharToUpper()}";
                collaborators.Add(model);
            }
            return collaborators;
        }
        catch (Exception e)
        {   // Do not remove those prints because they're used to debug AD requests errors !
            Console.WriteLine(e);
            Console.WriteLine("///////////////////////// AD DEBUGGING /////////////////////////");
            Console.WriteLine(e.InnerException);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all the users in the DirectoryEntries and inserts missing ones in the app db.
    /// </summary>
    private async Task CheckUsersAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var savedEmails = await db.Users.AsNoTracking().Select(x => x.Email).ToListAsync();
        var toAdd = DirectoryEntries.Where(x => !savedEmails.Contains(x.Email, StringComparer.OrdinalIgnoreCase)).ToList();
        
        await db.Users.AddRangeAsync(toAdd);
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
                user.Role = UserRole.Admin;
            }
        }
        
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }
}