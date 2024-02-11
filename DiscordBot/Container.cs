using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordBot
{
    public class Container
    {
        private readonly DiscordSocketClient _client;

        private readonly CommandHandler _commandHandler;

        private readonly InteractionHandler _interactionHandler;

        public Container(CommandHandler commandHandler, InteractionHandler interactionHandler)
        {
            _commandHandler = commandHandler;
            _interactionHandler = interactionHandler;
            _client = new DiscordSocketClient();
        }

        public async Task RunBotAsync()
        {
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
            _client.SlashCommandExecuted += _commandHandler.Execute;
            _client.ButtonExecuted += _interactionHandler.HandleButtonPress;

            await Task.Delay(-1);
        }

        public async Task Client_Ready(DiscordSocketClient client)
        {
            Console.WriteLine("Parancsok betöltése...");
            await _commandHandler.RegisterCommand(client, "coldest", "Leghidegebb hőmérséklet a következő 5 napban", new SlashCommandOptionBuilder()
                .WithName("település")
                .WithType(ApplicationCommandOptionType.String)
                .WithDescription("településnév")
                .WithRequired(true));
            await _commandHandler.RegisterCommand(client, "dice", "Kockajáték indítása", new SlashCommandOptionBuilder()
                .WithName("target")
                .WithType(ApplicationCommandOptionType.Integer)
                .WithDescription("Ennyi pontot kell elérni a nyeréshez (20-100)")
                .WithRequired(false));
            await _commandHandler.RegisterCommand(client, "hottest", "Legmelegebb hőmérséklet a következő 5 napban", new SlashCommandOptionBuilder()
                .WithName("település")
                .WithType(ApplicationCommandOptionType.String)
                .WithDescription("településnév")
                .WithRequired(true));
            await _commandHandler.RegisterCommand(client, "nextclear", "A legkorábbi időpontot mutatja meg, mikor derűs idő várható", new SlashCommandOptionBuilder()
                .WithName("település")
                .WithType(ApplicationCommandOptionType.String)
                .WithDescription("településnév")
                .WithRequired(true));
            await _commandHandler.RegisterCommand(client, "nextrain", "A legkorábbi időpontot mutatja meg, mikor esős idő várható", new SlashCommandOptionBuilder()
                .WithName("település")
                .WithType(ApplicationCommandOptionType.String)
                .WithDescription("településnév")
                .WithRequired(true));
            await _commandHandler.RegisterCommand(client, "nextsnow", "A legkorábbi időpontot mutatja meg, mikor havazás várható", new SlashCommandOptionBuilder()
                .WithName("település")
                .WithType(ApplicationCommandOptionType.String)
                .WithDescription("településnév")
                .WithRequired(true));
            await _commandHandler.RegisterCommand(client, "pong", "Debug Feature");
            await _commandHandler.RegisterCommand(client, "wr", "Időjárás lekérdezése", new SlashCommandOptionBuilder().WithName("település")
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
}