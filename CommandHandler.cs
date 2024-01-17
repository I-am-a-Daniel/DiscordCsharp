using Discord;
using Discord.Net;
using Discord.WebSocket;

public class CommandHandler
{
    public static async Task Execute(SocketSlashCommand command)
    {
        switch (command.Data.Name)
        {
            case "coldest":
                await command.RespondAsync(WeatherHandler.GetColdestTemperature((string)command.Data.Options.First().Value)); break;
            case "dice":
                await DiceGameManager.HandleDiceCommand(command); break;        //Ez így valóban nem szép. De működik.
            case "hottest":
                await command.RespondAsync(WeatherHandler.GetHottestTemperature((string)command.Data.Options.First().Value)); break;
            case "nextclear":
            //case "nextsun":               //TODO: Implement alias handling in RegisterCommand()
                await command.RespondAsync(WeatherHandler.GetNextClear((string)command.Data.Options.First().Value)); break;
            case "nextrain":
                await command.RespondAsync(WeatherHandler.GetNextRain((string)command.Data.Options.First().Value)); break;
            case "nextsnow":
                await command.RespondAsync(WeatherHandler.GetNextSnow((string)command.Data.Options.First().Value)); break;
            case "pong":
                await command.RespondAsync("Pong"); break;
            case "wr":
                if (command.Data.Options.Count == 1) //Nincs forecast
                {
                    var response = WeatherHandler.GetWeatherDataForCity((string)command.Data.Options.First().Value);
                    if (response != null)
                    {
                        await command.RespondAsync(embed: WeatherHandler.GetWeatherDataForCity((string)command.Data.Options.First().Value).Build()); //FIXME: Reklamál, hogy possible null reference.
                    }
                    else
                    {
                        await command.RespondAsync("Nincs ilyen város");
                    }
                }
                else                                //Van forecast
                {
                    var temp = command.Data.Options.FirstOrDefault(param => param.Name == "forecast").Value;
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
    public static async Task RegisterCommand(string name, string description, params SlashCommandOptionBuilder[] options)
    {
        var commandBuilder = new SlashCommandBuilder()
            .WithName(name)
            .WithDescription(description);
        
        foreach (var option in options)
        {
            commandBuilder.AddOption(option);
        }

        try
        {
            await Program._client.CreateGlobalApplicationCommandAsync(commandBuilder.Build());
            Console.WriteLine($"DEBUG: {commandBuilder.Name} betöltve.");
        }
        catch (HttpException exception)
        {
            Console.WriteLine(exception.Message);
        }
    }
}

