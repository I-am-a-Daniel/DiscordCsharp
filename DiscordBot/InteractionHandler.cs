using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordBot;

public class InteractionHandler
{
    private readonly DiceGameManager _diceGameManager;

    public InteractionHandler(DiceGameManager diceGameManager)
    {
        _diceGameManager = diceGameManager;
    }

    public async Task HandleButtonPress(SocketMessageComponent component)
    {
        switch(component.Data.CustomId)
        {
            case "dice_join":
                await _diceGameManager.OnDiceJoinButtonClicked(component); break;
            case "dice_start":
                await _diceGameManager.OnDiceStartButtonClicked(component); break;
            case "dice_stop":
                await _diceGameManager.OnDiceStopButtonClicked(component); break;
            case "dice_throw":
                await _diceGameManager.OnDiceThrowButtonClicked(component); break;
        }
    }
}