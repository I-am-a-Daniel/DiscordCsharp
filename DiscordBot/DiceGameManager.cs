using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot
{
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

        private GamePhase _currentPhase = GamePhase.PHASE_STANDBY;
        private readonly List<Player> _players = new List<Player>();
        private readonly Random _rng = new Random();
        private int _activePlayer = 0;
        private int _oldPlayer = 0; // Átmenetileg itt tároljuk az előző playert, ha egyszerre írjuk ki a következőt és a mostanit
        private int _target = 0;

        private void ResetGame()
        {
            _currentPhase = GamePhase.PHASE_STANDBY;
            _players.Clear();
            _activePlayer = 0;
            _oldPlayer = 0;
            _target = 0;
        }

        private readonly ComponentBuilder _gameBuilder = new ComponentBuilder()
            .WithButton("🎲", "dice_throw", ButtonStyle.Success)
            .WithButton("🛑", "dice_stop", ButtonStyle.Danger);

        [Command("dice")]
        public async Task HandleDiceCommand(SocketSlashCommand command, int pts = 66)
        {
            var lobbyBuilder = new ComponentBuilder()
                .WithButton("Csatlakozás", "dice_join")
                .WithButton("Indítás", "dice_start", ButtonStyle.Success);
            switch (_currentPhase)
            {
                case GamePhase.PHASE_STANDBY:
                    await command.RespondAsync($"{command.User.GlobalName} új kockajátékot indítiott! A cél {pts} pont elérése", components: lobbyBuilder.Build());
                    Player player = new Player                            // Ez itt elvileg nem duplikálhatja a playert később, mivel amint lefut, phase-t váltunk, és onnantól már ellenőrzi, csatlakozott-e
                    {
                        name = command.User.GlobalName,
                        gathering = 0,
                        total = 0,
                    };
                    _players.Add(player);
                    _target = pts;
                    _currentPhase = GamePhase.PHASE_CREATING_LOBBY;
                    break;
                case GamePhase.PHASE_CREATING_LOBBY:
                    string lobbyMessage = $"Továbbra is lehet csatlakozni a játékhoz! Aktuális játékosok: ";
                    foreach (var plyr in _players) { lobbyMessage += $"{plyr.name}, "; }
                    await command.RespondAsync(lobbyMessage, components: lobbyBuilder.Build());
                    break;
                case GamePhase.PHASE_PLAYING:
                    await command.RespondAsync($"Már folyamatban van egy játék. {_players[_activePlayer].name} dobására várunk..."); // Lehetne rakni ehhez is dobós componentet, de nem biztos, hogy kell
                    break;
            }
        }
        public async Task OnDiceJoinButtonClicked(SocketMessageComponent component)
        {
            if (!_players.Any(player => player.name == component.User.GlobalName))
            {
                Player newPlayer = new Player
                {
                    name = component.User.GlobalName,
                    gathering = 0,
                    total = 0
                };
                _players.Add(newPlayer);
                await component.RespondAsync($"{newPlayer.name} csatlakozott a játékhoz.");
            }
            else
            {
                await component.RespondAsync($"Már csatlakoztál a játékhoz!", ephemeral: true);
            }
        }
        public async Task OnDiceStartButtonClicked(SocketMessageComponent component)
        {
            if (_players.Count >= 1)
            {
                _activePlayer = _rng.Next(0, _players.Count);
                _currentPhase = GamePhase.PHASE_PLAYING;

                await component.Message.ModifyAsync(msg =>
                {
                    msg.Content = $"Kezdődik a kocskajáték!\nA játékot {_players[_activePlayer].name} kezdi";
                    msg.Components = _gameBuilder.Build();
                });
                await component.DeferAsync();
            }
            else
            {
                await component.RespondAsync($"Még nincs elegendő játékos az indításhoz", ephemeral: true);
            }
        }
        public async Task OnDiceThrowButtonClicked(SocketMessageComponent component)
        {

            if (!_players.Any(player => player.name == component.User.GlobalName))
            {
                await component.RespondAsync($"Nem vagy része ennek a kockajátéknak", ephemeral: true);
            }
            else if (component.User.GlobalName != _players[_activePlayer].name)
            {
                await component.RespondAsync($"Nem te következel", ephemeral: true);
            }
            else
            {

                int currentThrow = _rng.Next(1, 7);
                if (currentThrow != 6)
                {
                    _players[_activePlayer].gathering += currentThrow;
                    if (_players[_activePlayer].gathering + _players[_activePlayer].total < _target)
                    {
                        await component.Message.ModifyAsync(msg =>
                        {
                            msg.Content = $"A dobott szám: {currentThrow}, eddigi pontszám: {_players[_activePlayer].gathering} | Összesen: {_players[_activePlayer].gathering + _players[_activePlayer].total}";
                            msg.Components = _gameBuilder.Build();
                        });
                        await component.DeferAsync();   // Mert ha csak editelgetem az előző üzenetet, várni fog vmi responseot is, ami ilyenkor nincs.
                    }
                    else            //Játék vége handling ide
                    {
                        string endresponse = $"A dobott szám: {currentThrow}, eddigi pontszám: {_players[_activePlayer].gathering} | Összesen: {_players[_activePlayer].gathering + _players[_activePlayer].total}\nElérted a {_target} pontot, győztél!\nA végeredmény: ";
                        _players[_activePlayer].total += _players[_activePlayer].gathering;
                        _players[_activePlayer].gathering = 0;
                        foreach (var plyr in _players)
                        {
                            endresponse += $"{plyr.name}: {plyr.total} pont | ";
                        }
                        await component.Message.ModifyAsync(msg =>
                        {
                            msg.Content = endresponse;
                            msg.Components = null;
                        });
                        ResetGame();
                    }
                }
                else
                {
                    _oldPlayer = _activePlayer;
                    _players[_activePlayer].gathering = 0;
                    if (_activePlayer + 1 == _players.Count)
                    {
                        _activePlayer = 0;
                    }
                    else
                    {
                        _activePlayer++;
                    }
                    await component.Message.ModifyAsync(msg =>
                    {
                        msg.Content = $"A dobott szám: 6 - elvesztetted a pontjaidat. Biztos pontjaid száma: {_players[_oldPlayer].total}";
                        msg.Components = null;
                    });
                    await component.RespondAsync($"{_players[_activePlayer].name} következik. Eddig {_players[_activePlayer].total} pontot gyűjtött.", components: _gameBuilder.Build());
                }
            }
        }
        public async Task OnDiceStopButtonClicked(SocketMessageComponent component)
        {

            if (!_players.Any(player => player.name == component.User.GlobalName))
            {
                await component.RespondAsync($"Nem vagy része ennek a kockajátéknak", ephemeral: true);
            }
            else if (component.User.GlobalName != _players[_activePlayer].name)
            {
                await component.RespondAsync($"Nem te következel", ephemeral: true);
            }
            else
            {
                _players[_activePlayer].total += _players[_activePlayer].gathering;
                _players[_activePlayer].gathering = 0;
                _oldPlayer = _activePlayer;
                if (_activePlayer + 1 == _players.Count)
                {
                    _activePlayer = 0;
                }
                else
                {
                    _activePlayer++;
                }
                await component.Message.ModifyAsync(msg =>
                {
                    msg.Content = $"{_players[_oldPlayer].name} elmentette a pontjait, eddig {_players[_oldPlayer].total}-t szerzett.";
                    msg.Components = null;
                });
                await component.RespondAsync($"{_players[_activePlayer].name} következik. Eddig {_players[_activePlayer].total} pontot gyűjtött.", components: _gameBuilder.Build());
            }
        }
    }
}