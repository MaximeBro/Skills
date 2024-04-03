using System.DirectoryServices;
using LdapForNet;
using LdapForNet.Native;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Models.Enums;

namespace Skills.Services;


public class ActiveDirectoryService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public readonly List<string> UsersEntries = new();

    public async Task InitAsync()
    {
        UsersEntries.AddRange(GetADUsers());
        await CheckUsersAsync();
        await SetRootsAsync();
    }
    
    /// <summary>
    /// Retrieves all the users in the local AD.
    /// </summary>
    /// <returns>All the collaborators registered in the local AD.</returns>
    private List<string> GetADUsers()
    {
        try
        {
            var emails = new List<string>();
            
            var connection = new LdapConnection();
            var ip = configuration.GetSection("DirectoryServices")["ip"];
            connection.Connect(ip, 389);
            connection.Bind(Native.LdapAuthType.Negotiate, new LdapCredential()
            {
                UserName = configuration.GetSection("DirectoryServices")["login"],
                Password = configuration.GetSection("DirectoryServices")["password"]
            });
            
            var searchBase = "dc=sasp,dc=local";
            var searchFilter = "(objectClass=User)";

            var entries = connection.Search(searchBase, searchFilter, null, Native.LdapSearchScope.LDAP_SCOPE_SUB);
            foreach (var entry in entries)
            {
                var directoryEntry = entry.ToDirectoryEntry();
                var keys = directoryEntry.Attributes.Select(x => x.Name).ToList();
                if (directoryEntry.Attributes.TryGetValue("mail", out var mail))
                {
                    var emailAddress = mail.GetValue<string>() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(emailAddress) && emailAddress.EndsWith("@sasp.fr"))
                    {
                        emails.Add(emailAddress);
                    }
                }

                if (directoryEntry.Attributes.TryGetValue("displayName", out var name))
                {
                    
                }
            }
            connection.Dispose();
            return emails;
        }
        catch (Exception e)
        {
            Console.WriteLine("An unexpected error occured about the local AD connection ! See inner exception below :");
            Console.WriteLine(e);
        }

        return new();
    }

    /// <summary>
    /// Retrieves all the users in the DirectoryEntries and inserts missing ones in the app db.
    /// </summary>
    private async Task CheckUsersAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var savedEmails = await db.Users.AsNoTracking().Select(x => x.Email).ToListAsync();
        var toAdd = UsersEntries.Where(x => !savedEmails.Contains(x, StringComparer.OrdinalIgnoreCase)).ToList();

        // await db.Users.AddRange(toAdd);
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