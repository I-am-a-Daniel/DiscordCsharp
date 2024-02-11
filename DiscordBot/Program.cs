using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<WeatherClient>()
            .AddSingleton<DiceGameManager>()
            .AddSingleton<WeatherHandler>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<InteractionHandler>()
            .AddSingleton<Container>()
            .BuildServiceProvider();

        var container = serviceProvider.GetRequiredService<Container>();

        await container.RunBotAsync();
    }
}