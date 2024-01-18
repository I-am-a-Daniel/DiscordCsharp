﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class DiceGameManager
{
    public class Player
    {
        public string name { get; set; }
        public int gathering { get; set; }
        public int total { get; set; }
    }

    public enum GamePhase
    {
        PHASE_STANDBY,              // Nincs folyamatban játék
        PHASE_CREATING_LOBBY,       // Lehet csatlakozni a játékhoz
        PHASE_PLAYING               // Folyamatban van a játék
    }

    public static GamePhase current_phase = GamePhase.PHASE_STANDBY;
    public static List<Player> players = new List<Player>();
    public static Random rng = new Random();
    public static int activePlayer = 0;
    public static int oldPlayer = 0; // Átmenetileg itt tároljuk az előző playert, ha egyszerre írjuk ki a következőt és a mostanit

    public static void ResetGame()
    {
        current_phase = GamePhase.PHASE_STANDBY;
        players.Clear();
        activePlayer = 0;
        oldPlayer = 0;
    }

    public static ComponentBuilder gameBuilder = new ComponentBuilder()
        .WithButton("🎲", "dice_throw", ButtonStyle.Success)
        .WithButton("🛑", "dice_stop", ButtonStyle.Danger);

    [Command("dice")]
    public static async Task HandleDiceCommand(SocketSlashCommand command)  
    {
        var lobbyBuilder = new ComponentBuilder()
            .WithButton("Csatlakozás", "dice_join")
            .WithButton("Indítás", "dice_start", ButtonStyle.Success);
        switch (current_phase)
        {
            case GamePhase.PHASE_STANDBY:
                await command.RespondAsync($"{command.User.GlobalName} új kockajátékot indítiott!", components: lobbyBuilder.Build());
                Player player = new Player                            // Ez itt elvileg nem duplikálhatja a playert később, mivel amint lefut, phase-t váltunk, és onnantól már ellenőrzi, csatlakozott-e
                {
                    name = command.User.GlobalName,
                    gathering = 0,
                    total = 0,
                };
                players.Add(player);
                current_phase = GamePhase.PHASE_CREATING_LOBBY;
                break;
            case GamePhase.PHASE_CREATING_LOBBY:
                string lobbyMessage = $"Továbbra is lehet csatlakozni a játékhoz! Aktuális játékosok: ";
                foreach (var plyr in players) { lobbyMessage += $"{plyr.name}, "; }
                await command.RespondAsync(lobbyMessage, components: lobbyBuilder.Build());
                break;
            case GamePhase.PHASE_PLAYING:
                await command.RespondAsync($"Már folyamatban van egy játék. {players[activePlayer].name} dobására várunk..."); // Lehetne rakni ehhez is dobós componentet, de nem biztos, hogy kell
                break;
        }
    }
    public static async Task OnDiceJoinButtonClicked(SocketMessageComponent component)
    {
        if (!players.Any(player => player.name == component.User.GlobalName))
        {
            Player newPlayer = new Player
            {
                name = component.User.GlobalName,
                gathering = 0,
                total = 0
            };

            players.Add(newPlayer);
        }
        else
        {
            await component.RespondAsync($"Már csatlakoztál a játékhoz!", ephemeral: true);
        }
    }
    public static async Task OnDiceStartButtonClicked(SocketMessageComponent component)
    {
        if(players.Count > 1 || players.Count == 1)
        {
            activePlayer = rng.Next(0, players.Count);
            current_phase = GamePhase.PHASE_PLAYING;
            await component.RespondAsync($"Kezdődik a kocskajáték!\nA játékot {players[activePlayer].name} kezdi", components: gameBuilder.Build());
            await component.Message.DeleteAsync();
        }
        else
        {
            await component.RespondAsync($"Még nincs elegendő játékos az indításhoz", ephemeral: true);
        }
    }
    public static async Task OnDiceThrowButtonClicked(SocketMessageComponent component)
    {

        if (!players.Any(player => player.name == component.User.GlobalName))
        {
            await component.RespondAsync($"Nem vagy része ennek a kockajátéknak", ephemeral : true);
        }
        else if (component.User.GlobalName != players[activePlayer].name)
        {
            await component.RespondAsync($"Nem te következel", ephemeral: true);
        }
        else
        {

            int current_throw = rng.Next(1, 7);
            if(current_throw != 6)
            {
                players[activePlayer].gathering += current_throw;
                if (players[activePlayer].gathering + players[activePlayer].total < 66)
                {
                    await component.RespondAsync($"A dobott szám: {current_throw}, eddigi pontszám: {players[activePlayer].gathering} | Összesen: {players[activePlayer].gathering + players[activePlayer].total}", components: gameBuilder.Build());
                }
                else            //Játék vége handling ide
                {
                    string endresponse = $"A dobott szám: {current_throw}, eddigi pontszám: {players[activePlayer].gathering} | Összesen: {players[activePlayer].gathering + players[activePlayer].total}\nElérted a 66 pontot, győztél!\nA végeredmény: ";
                    players[activePlayer].total += players[activePlayer].gathering;
                    players[activePlayer].gathering = 0;
                    foreach (var plyr in players)
                    {
                        endresponse += $"{plyr.name}: {plyr.total} pont | ";
                    }
                    await component.RespondAsync(endresponse);
                    ResetGame();
                }
            }
            else
            {
                oldPlayer = activePlayer;
                players[activePlayer].gathering = 0;
                if(activePlayer + 1 == players.Count)
                {
                    activePlayer = 0;
                }
                else
                {
                    activePlayer++;
                }
                await component.RespondAsync($"A dobott szám: 6 - elvesztetted a pontjaidat. Biztos pontjaid száma: {players[oldPlayer].total}\n{players[activePlayer].name} következik.", components: gameBuilder.Build());
            
            }
            await component.Message.DeleteAsync();      // Ezáltal _elméletileg_ nem lehet bugoltatni az előző dobások reakcióit.
        }
    }
    public static async Task OnDiceStopButtonClicked(SocketMessageComponent component)
    {

        if (!players.Any(player => player.name == component.User.GlobalName))
        {
            await component.RespondAsync($"Nem vagy része ennek a kockajátéknak", ephemeral: true);
        }
        else if (component.User.GlobalName != players[activePlayer].name)
        {
            await component.RespondAsync($"Nem te következel", ephemeral: true);
        }
        else
        {
            players[activePlayer].total += players[activePlayer].gathering;
            players[activePlayer].gathering = 0;
            oldPlayer = activePlayer;
            if (activePlayer + 1 == players.Count)
            {
                activePlayer = 0;
            }
            else
            {
                activePlayer++;
            }
            await component.RespondAsync($"Elmentetted a pontjaidat - összesen {players[oldPlayer].total}-t szereztél.\n{players[activePlayer].name} következik.", components: gameBuilder.Build());
            await component.Message.DeleteAsync();
        }
    }
}