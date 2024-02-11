using System.Net;

namespace DiscordBot
{

    public class WeatherClient
    {
        private static string apikey = Keys.owm;

        public string GetCurrentWeatherJson(string city)
        {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apikey}&lang=hu";
            var json = new WebClient().DownloadString(url);
            return json;
        }

        public string GetForecastJson(string city)
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
}