using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        var serviceProvider = new ServiceCollection()
            .Configure<OWMSettings>(options => configuration.GetSection("OWM").Bind(options))
            .Configure<DiscordSettings>(options => configuration.GetSection("Discord").Bind(options))
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