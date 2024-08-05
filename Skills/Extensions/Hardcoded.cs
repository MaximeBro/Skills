using System.Globalization;
using MudBlazor;

namespace Skills.Extensions;

internal static class Hardcoded
{
    public static CultureInfo French => new CultureInfo("fr-FR");
    public static CultureInfo English => new CultureInfo("en-US");

    public const string SoftSkill = "SOFT-SKILL";
    
    public static DialogOptions DialogOptions => new DialogOptions
    {
        ClassBackground = "chrome-bg",
        CloseOnEscapeKey = true,
        DisableBackdropClick = false,
        CloseButton = true,
        NoHeader = true
    };
    
    public static Action<SnackbarOptions> SnackbarOptions = options =>
    {
        options.ShowCloseIcon = false;
        options.VisibleStateDuration = 1500;
        options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
    };
}