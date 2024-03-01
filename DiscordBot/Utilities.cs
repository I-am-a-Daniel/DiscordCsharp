using System.Globalization;

public class Utilities
{
    public static string Timestamp2String(double timestamp)                 //TODO: Untested
    {
        System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        dateTime = dateTime.AddSeconds(timestamp);
        string? str = dateTime.ToString("yyyy. MMMM dd. HH:mm", CultureInfo.CreateSpecificCulture("hu")) ?? "Ismeretlen időpont";   //Ha magyar nyelvű gépről fut, nem kell hardcodeolni
        return str;
    }
}

