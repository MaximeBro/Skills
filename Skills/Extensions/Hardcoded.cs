using System.Globalization;
using MudBlazor;

namespace Skills.Extensions;

public static class Hardcoded
{
    public static CultureInfo French => new CultureInfo("fr-FR");
    
    public static DialogOptions DialogOptions => new DialogOptions
    {
        ClassBackground = "chrome-bg",
        CloseOnEscapeKey = true,
        DisableBackdropClick = false,
        CloseButton = true,
        NoHeader = true
    };
}