using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;

class Program
{
    private DiscordSocketClient _client;

    static async Task Main(string[] args)
    {
        var program = new Program();
        await program.RunBotAsync();
    }

    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, "");
        await _client.StartAsync();
        _client.Ready += () =>
        {
            //Console.WriteLine("Bot successfully connected");
            Client_Ready(); // Valami warningot ír rá mert nincs await, majd meglátjuk h baj-e
            return Task.CompletedTask;
        };
        _client.SlashCommandExecuted += CommandHandler;
        
        await Task.Delay(-1);
    }

    public async Task Client_Ready()
    {
        Console.WriteLine("Parancsok betöltése...");
        //Pong command
        var pong_command = new SlashCommandBuilder()
            .WithName("ping")                                                           //Parancsok mindig kisbetűvel
            .WithDescription("Debug function");
        try
        {
            await _client.CreateGlobalApplicationCommandAsync(pong_command.Build());    // Geci mi ez a function
        }
        catch (HttpException exception)
        {
            Console.WriteLine(exception.Message);
        }
        //Weather command
        var weather_command = new SlashCommandBuilder()
            .WithName("wr")
            .WithDescription("Weather information")
            .AddOption("city", ApplicationCommandOptionType.String, "City name", isRequired: true)
            .AddOption("forecast", ApplicationCommandOptionType.Integer, "Forecast in hours", isRequired: false);
        try
        {
            await _client.CreateGlobalApplicationCommandAsync(weather_command.Build());
        }
        catch (HttpException exception)
        {
            Console.Write(exception.Message);
        }
        Console.WriteLine("Parancsok betöltésének vége.");
    }

    public async Task CommandHandler(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "ping":
                await command.RespondAsync("Pong"); break;
            case "wr":
                if(command.Data.Options.Count == 1) //Nincs forecast
                {
                    await command.RespondAsync(embed: WeatherHandler.GetWeatherDataForCity((string)command.Data.Options.First().Value).Build());
                }
                else                                //Van forecast
                {
                    var temp = command.Data.Options.FirstOrDefault(param => param.Name == "forecast").Value;  //FIXME: Conversion error
                    int hours = Convert.ToInt32(temp);
                    if (hours > 100 || hours < 0)
                    {
                        await command.RespondAsync("3 és 100 óra közötti időtávot adj meg.", ephemeral: true);
                    }
                    else
                        await command.RespondAsync(embed: WeatherHandler.GetWeatherForecastForCity((string)command.Data.Options.First().Value, hours).Build());
                }
                break;
            default:
                await command.RespondAsync("Valami nem jó"); break;
        }
    }
    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }
}
public class Utilities
{
    public static string Timestamp2String(double timestamp)                 //TODO: Untested
    {
        System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
        dateTime = dateTime.AddSeconds(timestamp);
        string? str = dateTime.ToString("yyyy. MMMM dd. HH:mm") ?? "Ismeretlen időpont";
        return str;
    }
}
public class Weather
{
    private static string apikey = "";
    public static string GetCurrentWeatherJson(string city)
    {
            string url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={apikey}&lang=hu";
            var json = new WebClient().DownloadString(url);
            return json;
    }
    public static string GetForecastJson(string city)
    {
        string url = $"http://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apikey}&lang=hu";
        var json = new WebClient().DownloadString(url);
        return json;
    }
}

public class WeatherHandler
{
    public static EmbedBuilder GetWeatherDataForCity(string city)       //TODO: Error handling, e.g. Non-existing location
    {
        string json = Weather.GetCurrentWeatherJson(city);
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        Location location = new Location();
        location.Temperature = Convert.ToInt32(root.GetProperty("main").GetProperty("temp").GetDouble() - 273);
        location.Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();
        location.WindSpeed = Convert.ToInt32(root.GetProperty("wind").GetProperty("speed").GetDouble() * 3.6);
        location.Name = root.GetProperty("name").ToString() ?? city;
        location.Weather = char.ToUpper(root.GetProperty("weather")[0].GetProperty("description").ToString()[0]) + root.GetProperty("weather")[0].GetProperty("description").ToString()[1..]; //Édes istenem
        location.WindDirection = GetWindDirString(root.GetProperty("wind").GetProperty("deg").GetInt32()) ?? "Változó irányú"; // Ritkán, de van olyan, hogy szélirány nincs a jsonben, szélerősség viszont igen
        location.Icon = $"https://openweathermap.org/img/wn/{root.GetProperty("weather")[0].GetProperty("icon")}@2x.png";
        //string response = $"Időjárás {location.Name} területén: {location.Weather}, {location.Temperature} °C.";
        var response = new EmbedBuilder()
            .WithTitle($"Időjárás {location.Name} területén")
            .WithDescription($"{location.Weather}, {location.Temperature} °C.\n {location.WindDirection} szél, {location.WindSpeed} km/h.")
            .WithThumbnailUrl(location.Icon);
        return response;
    }

    public static EmbedBuilder GetWeatherForecastForCity(string city, int hour)
    {
        string json = Weather.GetForecastJson(city);
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        Location location = new Location();
        int n = hour / 3;
        location.Temperature = Convert.ToInt32(root.GetProperty("list")[n].GetProperty("main").GetProperty("temp").GetDouble() - 273);
        location.Weather = char.ToUpper(root.GetProperty("list")[n].GetProperty("weather")[0].GetProperty("description").ToString()[0]) + root.GetProperty("list")[n].GetProperty("weather")[0].GetProperty("description").ToString()[1..]; //Milyen kár, hogy nincs String.Title()
        location.WindForecast = GetWindSpeedText(Convert.ToInt32(root.GetProperty("list")[n].GetProperty("wind").GetProperty("speed").GetDouble() * 3.6)) ?? "Szélcsend";
        location.Icon = $"https://openweathermap.org/img/wn/{root.GetProperty("list")[n].GetProperty("weather")[0].GetProperty("icon")}@2x.png";
        location.Name = root.GetProperty("city").GetProperty("name").ToString();
        string? stamp = Utilities.Timestamp2String(double.Parse(root.GetProperty("list")[n].GetProperty("dt").ToString())) ?? "Ismeretlen időpont";     //!!Ez dt és nem dt_txt mint a python botban, jó volt erre is elbaszni fél órát. - TODO kitalálni, hogy ott miért működött egyáltalán
        var response = new EmbedBuilder()
            .WithTitle($"Előrejelzés {location.Name} területére")
            .WithDescription($"{location.Weather}, {location.Temperature} °C. {location.WindForecast}.")
            .WithThumbnailUrl(location.Icon)
            .WithFooter($"Érvényesség: {stamp} + 3 óra");
        return response;
    }

    public class Location
    {
        public string? Name { get; set; }
        public string? Weather { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set;}
        public double WindSpeed { get; set; }
        public string? WindDirection { get; set; }
        public string? Icon { get; set; }
        public string? WindForecast { get; set; }
    }

    public static string GetWindDirString(int deg)
    {
        string direction = "";
        if (deg > 337 || deg <= 22)
            direction = "Északi";
        else if (deg > 22 && deg <= 67)
            direction = "Északkeleti";
        else if (deg > 67 && deg <= 112)
            direction = "Keleti";
        else if (deg > 112 && deg <= 157)
            direction = "Délkeleti";
        else if (deg > 157 && deg <= 202)
            direction = "Déli";
        else if (deg > 202 && deg <= 247)
            direction = "Délnyugati";
        else if (deg > 247 && deg <= 292)
            direction = "Nyugati";
        else if (deg > 292 && deg <= 337)
            direction = "Északnyugati";
        return direction;
    }
    public static string GetWindSpeedText(int speed)
    {
        string text = "";
        if (speed < 5) text = "Szélcsend";
        else if (speed >= 5 && speed < 15) text = "Enyhe szél";
        else if (speed >= 15 && speed < 25) text = "Mérsékelt szél";
        else if (speed >= 25 && speed < 35) text = "Élénk szél";
        else if (speed >= 35) text = "Viharos szél";
        return text;
    }
}

