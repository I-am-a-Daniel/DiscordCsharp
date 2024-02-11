using System;
using System.Text.Json;
using Discord;
using DiscordBot.Extensions;

namespace DiscordBot;

public class WeatherHandler
{
    private readonly WeatherClient _weatherClient = new();
    
    public string GetColdestTemperature(string city)
    {
        int temperature = 100;
        int n = 0;
        int temperature_temp;
        string? json = _weatherClient.GetForecastJson(city);
        if (json == null) { return new string("Nincs ilyen város"); }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        for (int i = 0; i < root.GetProperty("cnt").GetInt32(); i++)
        {
            temperature_temp = Convert.ToInt32(root.GetProperty("list")[i].GetProperty("main").GetProperty("temp").GetDouble() - 273);
            if (temperature_temp < temperature)
            {
                temperature = temperature_temp;
                n = i;
            }
        }

        var stamp = double.Parse(root.GetProperty("list")[n].GetProperty("dt").ToString()).ToHungarianForm();
        return new string ($"A következő 5 napban {stamp}-kor lesz a leghidegebb, {temperature} °C.");

    }
    public string GetHottestTemperature(string city)
    {
        int temperature = -100;
        int n = 0;
        int temperature_temp;
        string? json = _weatherClient.GetForecastJson(city);
        if (json == null) { return new string("Nincs ilyen város"); }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        for (int i = 0; i < root.GetProperty("cnt").GetInt32(); i++)
        {
            temperature_temp = Convert.ToInt32(root.GetProperty("list")[i].GetProperty("main").GetProperty("temp").GetDouble() - 273);
            if (temperature_temp > temperature)
            {
                temperature = temperature_temp;
                n = i;
            }
        }

        var stamp = double.Parse(root.GetProperty("list")[n].GetProperty("dt").ToString()).ToHungarianForm();
        return new string($"A következő 5 napban {stamp}-kor lesz a legmelegebb, {temperature} °C.");

    }
    public string GetNextClear(string city)
    {
        string? json = _weatherClient.GetForecastJson(city);
        if (json == null) { return new string("Nincs ilyen város"); }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        for (int i = 0; i < root.GetProperty("cnt").GetInt32(); i++)
        {
            if (root.GetProperty("list")[i].GetProperty("weather")[0].GetProperty("main").ToString() == "Clear")
            {
                var stamp = double.Parse(root.GetProperty("list")[i].GetProperty("dt").ToString()).ToHungarianForm();
                return new string($"A következő derűs idő {stamp}-kor várható.");
            }

        }
        return "Nem várható derűs idő a következő öt napban.";
    }
    public string GetNextRain(string city)
    {
        string? json = _weatherClient.GetForecastJson(city);
        if (json == null) { return new string("Nincs ilyen város"); }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        for (int i = 0; i < root.GetProperty("cnt").GetInt32(); i++)
        {
            if (root.GetProperty("list")[i].GetProperty("weather")[0].GetProperty("main").ToString() == "Rain")
            {
                var stamp = double.Parse(root.GetProperty("list")[i].GetProperty("dt").ToString()).ToHungarianForm();
                return new string($"A következő eső {stamp}-kor várható.");
            }

        }
        return "Nem várható eső a következő öt napban.";
    }
    public string GetNextSnow(string city)
    {
        string? json = _weatherClient.GetForecastJson(city);
        if (json == null) { return new string("Nincs ilyen város"); }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        for (int i = 0; i < root.GetProperty("cnt").GetInt32(); i++)
        {
            if (root.GetProperty("list")[i].GetProperty("weather")[0].GetProperty("main").ToString() == "Snow")
            {
                var stamp = double.Parse(root.GetProperty("list")[i].GetProperty("dt").ToString()).ToHungarianForm();
                return new string($"A következő havazás {stamp}-kor várható.");
            }

        }
        return "Nem várható havazás a következő öt napban.";
    }
    public EmbedBuilder? GetWeatherDataForCity(string city)
    {
        string json;
        try                                                            //EmbedBuilder miatt itt és a forecastnál egyelőre más errorkezelés lesz 
        {
            json = _weatherClient.GetCurrentWeatherJson(city); 
        }
        catch (System.Net.WebException)
        {
            return null;
        }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        Location location = new Location();
        //int code = root.GetProperty("cod").GetInt32();                //Tulajdonképpen nincs rá szükség
        location.Temperature = Convert.ToInt32(root.GetProperty("main").GetProperty("temp").GetDouble() - 273);
        location.Humidity = root.GetProperty("main").GetProperty("humidity").GetInt32();
        location.WindSpeed = Convert.ToInt32(root.GetProperty("wind").GetProperty("speed").GetDouble() * 3.6);
        location.Name = root.GetProperty("name").ToString() ?? city;
        location.Weather = char.ToUpper(root.GetProperty("weather")[0].GetProperty("description").ToString()[0]) + root.GetProperty("weather")[0].GetProperty("description").ToString()[1..]; //Édes istenem
        location.WindDirection = GetWindDirString(root.GetProperty("wind").GetProperty("deg").GetInt32()) ?? "Változó irányú"; // Ritkán, de van olyan, hogy szélirány nincs a jsonben, szélerősség viszont igen
        location.Icon = $"https://openweathermap.org/img/wn/{root.GetProperty("weather")[0].GetProperty("icon")}@2x.png";
        //string response = $"Időjárás {location.Name} területén: {location.Weather}, {location.Temperature} °C.";  //Ha kell majd stringként
        var response = new EmbedBuilder()
            .WithTitle($"Időjárás {location.Name} területén")
            .WithDescription($"{location.Weather}, {location.Temperature} °C.\n {location.WindDirection} szél, {location.WindSpeed} km/h.")
            .WithThumbnailUrl(location.Icon);
        return response;

    }

    public EmbedBuilder? GetWeatherForecastForCity(string city, int hour)
    {
        string json;// = Weather.GetForecastJson(city);
        try
        {
            json = _weatherClient.GetForecastJson(city);
        }
        catch (System.Net.WebException)
        {
            return null;
        }
        JsonDocument jsondoc = JsonDocument.Parse(json);
        JsonElement root = jsondoc.RootElement;
        Location location = new Location();
        int n = hour / 3;
        location.Temperature = Convert.ToInt32(root.GetProperty("list")[n].GetProperty("main").GetProperty("temp").GetDouble() - 273);
        location.Weather = char.ToUpper(root.GetProperty("list")[n].GetProperty("weather")[0].GetProperty("description").ToString()[0]) + root.GetProperty("list")[n].GetProperty("weather")[0].GetProperty("description").ToString()[1..]; //Milyen kár, hogy nincs String.Title()
        location.WindForecast = GetWindSpeedText(Convert.ToInt32(root.GetProperty("list")[n].GetProperty("wind").GetProperty("speed").GetDouble() * 3.6)) ?? "Szélcsend";
        location.Icon = $"https://openweathermap.org/img/wn/{root.GetProperty("list")[n].GetProperty("weather")[0].GetProperty("icon")}@2x.png";
        location.Name = root.GetProperty("city").GetProperty("name").ToString();
        string? stamp = double.Parse(root.GetProperty("list")[n].GetProperty("dt").ToString()).ToHungarianForm();     //!!Ez dt és nem dt_txt mint a python botban, jó volt erre is elbaszni fél órát. - TODO kitalálni, hogy ott miért működött egyáltalán
        var response = new EmbedBuilder()
            .WithTitle($"Előrejelzés {location.Name} területére")
            .WithDescription($"{location.Weather}, {location.Temperature} °C. {location.WindForecast}.")
            .WithThumbnailUrl(location.Icon)
            .WithFooter($"Érvényesség: {stamp} + 3 óra");
        return response;
    }

    private class Location
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

    private static string GetWindDirString(int deg)
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

    private static string GetWindSpeedText(int speed)
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