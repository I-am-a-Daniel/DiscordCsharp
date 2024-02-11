using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
class Program
{
    public static DiscordSocketClient _client;

    static async Task Main(string[] args)
    {
        var program = new Program();
        await program.RunBotAsync();
    }

    private async Task RunBotAsync()
    {
        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        Keys.Read();


        await _client.LoginAsync(TokenType.Bot, Keys.main);
        await _client.StartAsync();
        _client.Ready += async () =>
        {
            //Console.WriteLine("Bot successfully connected");
            // TODO test if this works
            await Client_Ready(_client);
        };
        _client.SlashCommandExecuted += CommandHandler.Execute;
        _client.ButtonExecuted += InteractionHandler.HandleButtonPress;
        
        await Task.Delay(-1);
    }

    public async Task Client_Ready(DiscordSocketClient client)
    {
        Console.WriteLine("Parancsok betöltése...");
        await CommandHandler.RegisterCommand(client, "coldest", "Leghidegebb hőmérséklet a következő 5 napban", new SlashCommandOptionBuilder()
            .WithName("település")
            .WithType(ApplicationCommandOptionType.String)
            .WithDescription("településnév")
            .WithRequired(true));
        await CommandHandler.RegisterCommand(client , "dice", "Kockajáték indítása", new SlashCommandOptionBuilder()
            .WithName("target")
            .WithType(ApplicationCommandOptionType.Integer)
            .WithDescription("Ennyi pontot kell elérni a nyeréshez (20-100)")
            .WithRequired(false));
        await CommandHandler.RegisterCommand(client, "hottest", "Legmelegebb hőmérséklet a következő 5 napban", new SlashCommandOptionBuilder()
            .WithName("település")
            .WithType(ApplicationCommandOptionType.String)
            .WithDescription("településnév")
            .WithRequired(true));
        await CommandHandler.RegisterCommand(client, "nextclear", "A legkorábbi időpontot mutatja meg, mikor derűs idő várható", new SlashCommandOptionBuilder()
            .WithName("település")
            .WithType(ApplicationCommandOptionType.String)
            .WithDescription("településnév")
            .WithRequired(true));
        await CommandHandler.RegisterCommand(client, "nextrain", "A legkorábbi időpontot mutatja meg, mikor esős idő várható", new SlashCommandOptionBuilder()
            .WithName("település")
            .WithType(ApplicationCommandOptionType.String)
            .WithDescription("településnév")
            .WithRequired(true));
        await CommandHandler.RegisterCommand(client, "nextsnow", "A legkorábbi időpontot mutatja meg, mikor havazás várható", new SlashCommandOptionBuilder()
            .WithName("település")
            .WithType(ApplicationCommandOptionType.String)
            .WithDescription("településnév")
            .WithRequired(true));
        await CommandHandler.RegisterCommand(client, "pong", "Debug Feature");
        await CommandHandler.RegisterCommand(client, "wr", "Időjárás lekérdezése", new SlashCommandOptionBuilder()
            .WithName("település")
            .WithType(ApplicationCommandOptionType.String)
            .WithDescription("településnév")
            .WithRequired(true),
            new SlashCommandOptionBuilder()
            .WithName("előrejelzés")
            .WithType(ApplicationCommandOptionType.Integer)
            .WithDescription("Hány óra múlva szeretnéd kérni az előrejelzést")
            .WithRequired(false));
        Console.WriteLine("Parancsok betöltésének vége.");
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }
}

