using System.Net;
using DiscordBot.Settings;
using Microsoft.Extensions.Options;

namespace DiscordBot
{

    public class WeatherClient
    {
        private readonly OWMSettings _owmSettings;

    public WeatherClient(IOptions<OWMSettings> owmSettings)
    {
        _owmSettings = owmSettings.Value;
    }

        public string GetCurrentWeatherJson(string city)
        {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_owmSettings.ApiKey}&lang=hu";
            var json = new WebClient().DownloadString(url);
            return json;
        }

        public string GetForecastJson(string city)
        {
            string url = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&appid={_owmSettings.ApiKey}&lang=hu";
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