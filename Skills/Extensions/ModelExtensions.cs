using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using Skills.Models;

namespace Skills.Extensions;

public static class ModelExtensions
{
    public static string FirstCharToUpper(this string @this) => string.Concat(@this[0].ToString().ToUpper(), @this.AsSpan(1));
    
    /// <summary>
    /// Get all the flags of the specified enum.
    /// </summary>
    /// <param name="this">An <see cref="Enum"/></param>
    /// <returns>An <see cref="IEnumerable{T}"/> of the given enum.</returns>
    public static IEnumerable<Enum> GetFlags(this Enum @this)
    {
        return Enum.GetValues(@this.GetType()).Cast<Enum>().Where(@this.HasFlag);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="this">Any <see cref="UserModel"/></param>
    /// <returns></returns>
    public static string GetTrigramme(this UserModel @this) => $"{@this.Name[0]}{@this.Name.Split(" ")[1].Substring(0, 2)}".ToUpper();

    /// <summary>
    /// Converts a <see cref="UserPrincipal"/> into a <see cref="UserModel"/> with the name and email of the user.
    /// </summary>
    /// <param name="this">The <see cref="UserPrincipal"/> to convert.</param>
    /// <returns>The converted user.</returns>
    [SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
    public static UserModel ToUserModel(this UserPrincipal @this)
    {
        return new UserModel
        {
            Name = $"{@this.EmailAddress.Replace("@sasp.fr", string.Empty).Split(".")[0].FirstCharToUpper()} {@this.EmailAddress.Replace("@sasp.fr", string.Empty).Split(".")[1].FirstCharToUpper()}",
            Email = @this.EmailAddress,
            Username = @this.EmailAddress.Replace("@sasp.fr", string.Empty),
            IsDisabled = @this.Enabled != null && (bool)!@this.Enabled
        };
    }
    
    /// <summary>
    /// Converts a list of <see cref="UserPrincipal"/> into a list of <see cref="UserModel"/> with the name and email of each user.
    /// </summary>
    /// <param name="this">The list of <see cref="UserPrincipal"/> to convert.</param>
    /// <returns>The converted users as a new list.</returns>
    [SuppressMessage("Interoperability", "CA1416:Valider la compatibilité de la plateforme")]
    public static List<UserModel> ToUserModel(this List<UserPrincipal> @this)
    {
        return @this.Select(x => x.ToUserModel()).ToList();
    }
}