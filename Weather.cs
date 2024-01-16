using System.Net;

public class Weather
{
    private static string apikey = Keys.owm;
    public static string GetCurrentWeatherJson(string city)
    {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apikey}&lang=hu";
            var json = new WebClient().DownloadString(url);
            return json;
    }
    public static string GetForecastJson(string city)
    {
        string url = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apikey}&lang=hu";
        try
        {
            var json = new WebClient().DownloadString(url);
            return json;
        }
        catch
        {
            return null;
        }
    }
}

