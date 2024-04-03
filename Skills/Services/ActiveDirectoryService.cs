using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Skills.Databases;
using Skills.Extensions;
using Skills.Models.Enums;
using SearchScope = System.DirectoryServices.Protocols.SearchScope;

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
            
            var endpoint = new LdapDirectoryIdentifier(configuration.GetSection("DirectoryServices")["ip"], false, false);
            var ldap = new LdapConnection(endpoint, new NetworkCredential(configuration.GetSection("DirectoryServices")["login"], configuration.GetSection("DirectoryServices")["password"]))
            {
                AuthType = AuthType.Negotiate
            };

            ldap.SessionOptions.ProtocolVersion = 3;
            ldap.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
            ldap.Timeout = TimeSpan.FromMinutes(1);
            ldap.Bind();

            var dn = "dc=sasp,dc=local";
            var filter = "(objectClass=User)";
            var request = new SearchRequest(dn, filter, SearchScope.Subtree);
            var result = ldap.SendRequest(request);
            if (result is SearchResponse response)
            {
                foreach (SearchResultEntry entry in response.Entries)
                {
                    if (entry.Attributes.Contains("mail"))
                    {
                        var email = entry.Attributes["mail"].GetValues(typeof(string))[0] as string;
                        if (!string.IsNullOrWhiteSpace(email)) emails.Add(email);
                    }
                }
            }
            
            ldap.Dispose();
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

        await db.Users.AddRangeAsync(toAdd.ToUserModels());
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