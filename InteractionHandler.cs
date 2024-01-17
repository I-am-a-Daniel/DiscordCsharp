using Discord.WebSocket;

public class InteractionHandler
{
    public static async Task HandleButtonPress(SocketMessageComponent component)
    {
        switch(component.Data.CustomId)
        {
            case "dice_join":
                await DiceGameManager.OnDiceJoinButtonClicked(component); break;
            case "dice_start":
                await DiceGameManager.OnDiceStartButtonClicked(component); break;
            case "dice_stop":
                await DiceGameManager.OnDiceStopButtonClicked(component); break;
            case "dice_throw":
                await DiceGameManager.OnDiceThrowButtonClicked(component); break;
        }
    }
}