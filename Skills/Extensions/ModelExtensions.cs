using System.Diagnostics.CodeAnalysis;
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

    public static IEnumerable<UserModel> ToUserModels(this IEnumerable<string> @this)
    {
        return @this.ToList().ConvertAll(x => new UserModel
        {
            Email = x,
            Username = x.Replace("@sasp.fr", string.Empty),
            Name = $"{x.Replace("@sasp.fr", string.Empty).Split(".")[0].FirstCharToUpper()} {x.Replace("@sasp.fr", string.Empty).Split(".")[1].FirstCharToUpper()}"
        });
    }
}