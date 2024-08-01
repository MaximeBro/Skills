namespace Skills.Extensions;

public static class DateExtensions
{
    public static string DisplayRemainingTime(this TimeSpan diff)
    {
        if (diff.Days > 365)
        {
            return $"{diff.Days/365} ANS";
        }
        
        if (diff.Days > 30)
        {
            return $"{diff.Days/30} MOIS";
        }
        
        if (diff.Days > 0)
        {
            return $"{diff.Days}J";
        }

        if (diff.Hours > 0)
        {
            return $"{diff.Hours}H";
        }

        return $"{diff.Minutes}M";
    }
}