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
    /// Creates the trigramme of a user using its Name property.
    /// </summary>
    /// <param name="this">Any <see cref="UserModel"/>.</param>
    /// <returns>The calculated trigramme of the given user.</returns>
    public static string GetTrigramme(this UserModel @this) => $"{@this.Name[0]}{@this.Name.Split(" ")[1].Substring(0, 2)}".ToUpper();

    /// <summary>
    /// Creates an "abstract" version of this <see cref="TypeLevel"/>.
    /// </summary>
    /// <param name="this">Any <see cref="TypeLevel"/>.</param>
    /// <returns>A <see cref="CommonTypeModel"/>.</returns>
    public static CommonTypeModel ToAbstract(this TypeLevel @this)
    {
        return new CommonTypeModel
        {
            Id = @this.Id,
            ParentId = @this.TypeId,
            Level = @this.Level,
            Value = @this.Value,
            IsSoftLevel = false
        };
    }
    
    /// <summary>
    /// Checks if the given date is older than the current date by the given interval of days.
    /// </summary>
    /// <param name="this">The datetime</param>
    /// <param name="days">The interval of days</param>
    /// <returns>True if it is older, false otherwise.</returns>
    public static bool IsOlderThan(this DateTime @this, int days) => (DateTime.Now - @this).TotalDays >= days;
    
    /// <summary>
    /// Creates an "abstract" version of this <see cref="SoftTypeLevel"/>.
    /// </summary>
    /// <param name="this">Any <see cref="SoftTypeLevel"/>.</param>
    public static CommonTypeModel ToAbstract(this SoftTypeLevel @this)
    {
        return new CommonTypeModel
        {
            Id = @this.Id,
            ParentId = @this.SkillId,
            Level = @this.Level,
            Value = @this.Value,
            IsSoftLevel = false
        };
    }
    
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