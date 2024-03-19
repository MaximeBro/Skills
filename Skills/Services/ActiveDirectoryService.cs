using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;

namespace Skills.Services;

[SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
public class ActiveDirectoryService
{
    private readonly IConfiguration _configuration;
    private readonly List<UserPrincipal> _directoryEntries = new();

    public ActiveDirectoryService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task InitAsync()
    {
        _directoryEntries.AddRange(GetActiveDirectoryUsers());
        return Task.CompletedTask;
    }
    
    private List<UserPrincipal> GetActiveDirectoryUsers()
    {
        var context = new PrincipalContext(ContextType.Domain, _configuration["ip"]);
        
        var searcher = new PrincipalSearcher(new UserPrincipal(context));
        var results = searcher.FindAll().Cast<UserPrincipal>().ToList();
        var collaborators = results.Where(x => !string.IsNullOrWhiteSpace(x.EmailAddress) && x.EmailAddress.EndsWith("@sasp.fr")).ToList();

        return collaborators;
    }
}