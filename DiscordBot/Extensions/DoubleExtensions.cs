using System;
using System.Globalization;

namespace DiscordBot.Extensions;

public static class DoubleExtensions
{
    public static string ToHungarianForm(this double timestamp)                 //TODO: Untested
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        dateTime = dateTime.AddSeconds(timestamp);
        return dateTime.ToString("yyyy. MMMM dd. HH:mm", CultureInfo.CreateSpecificCulture("hu"));   //Ha magyar nyelvű gépről fut, nem kell hardcodeolni
    }
}