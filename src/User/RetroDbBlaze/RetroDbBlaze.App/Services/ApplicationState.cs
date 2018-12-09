using Microsoft.AspNetCore.Blazor;
using Microsoft.Extensions.Logging;
using RetroDb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RetroDbBlaze.App.Services
{
    public class ApplicationState : IDisposable
    {
        public event Action OnStateChanged;        
        string username = "RetroDbClient";
        ChatClient client;
        private readonly HttpClient _http;
        private readonly ILogger<ApplicationState> _logger;

        public ApplicationState(HttpClient httpClient, ILogger<ApplicationState> logger)
        {
            _http = httpClient;
            _logger = logger;

            InitAppState();            
        }

        private void InitAppState()
        {
            bool result = false;
            Task.Run(async () =>
            {
                Console.WriteLine($"init app state");
                result = await _http.GetJsonAsync<bool>("/api/LaunchGame/Running");
                await PopulateStats();
                await PopulateSystems();
                await InitChatClient();
            }).ContinueWith(t =>
            {
                IsRocketLaunchRunning = result;
                Console.WriteLine($"Rocketlauncher running: {IsRocketLaunchRunning}");
                OnStateChanged?.Invoke();
            });
        }

        #region Properties

        public bool IsRocketLaunchRunning { get; set; }

        #region Collections
        public GameSearchOption GameSearchOption { get; set; } = new GameSearchOption("", 1, 10);
        public PagedResult<Game> GameSearchResults { get; private set; }
        public RetroDbStats RetroDbStats { get; private set; } = new RetroDbStats();
        public Game RunningGame { get; set; }
        public IReadOnlyList<GameSystem> Systems { get; private set; }
        public bool IsSearchRunning { get; private set; }
        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Launches the game through rocketlauncher
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public async Task LaunchGame(Game game)
        {
            IsRocketLaunchRunning = true;
            OnStateChanged?.Invoke();

            _logger.LogInformation($"Loading game: {game?.System?.Name} - {game?.FileName}");
            var gameUrl = "api/LaunchGame/" + game.Id;
            RunningGame = game;
            OnStateChanged?.Invoke();
            await _http.GetAsync(gameUrl); //TODO: Change to POST
        }

        public async Task NavigatePause(string rlCommand)
        {
            _logger.LogInformation($"Navigating Pause Command: {rlCommand}");
            var result = await _http.PostJsonAsync<int>($"api/launchgame/pause/nav/", rlCommand);
            _logger.LogInformation($"Navigated pause?: {result}");
        }

        public async Task PauseGame()
        {
            _logger.LogInformation("Pausing game");
            var result = await _http.GetJsonAsync<int>("api/launchgame/pause");
            _logger.LogInformation($"Game pause? {result}");
        }

        private async Task PopulateStats()
        {
            _logger.LogInformation("Populating Stats...");
            RetroDbStats = await _http.GetJsonAsync<RetroDbStats>(@"/api/stats");
            _logger.LogInformation($"Populated stats.");
        }

        /// <summary>
        /// Gets all the Game Systems, see <see cref="Systems"/>
        /// </summary>
        /// <returns></returns>
        public async Task PopulateSystems()
        {
            _logger.LogInformation("Populating Systems...");
            Systems = await _http.GetJsonAsync<List<GameSystem>>(@"/api/systems");
            Systems =  Systems.OrderBy(x => x.Name).ToList();
            _logger.LogInformation($"Populated Systems. {Systems?.Count}");
        }

        public async Task SearchGamesAsync(GameSearchOption gameSearchOption)
        {
            IsSearchRunning = true;
            OnStateChanged?.Invoke();

            GameSearchResults = await _http.PostJsonAsync<PagedResult<Game>>("api/search", gameSearchOption);

            IsSearchRunning = false;
            OnStateChanged?.Invoke();
        }

        public async Task QuitGame()
        {
            _logger.LogInformation("User sent QuitGame");
            var result = await _http.GetJsonAsync<int>("api/launchgame/quit");
            if (result > 0)
            {
                RunningGame = null;
                this.OnStateChanged?.Invoke();
            }
                
            _logger.LogInformation($"Game Quit? {result}");

        }

        #endregion

        #region Support methods

        async Task Disconnect()
        {
            await client.Stop();
            client.Dispose();
            client = null;
        }

        /// <summary>
        /// Start chat client
        /// </summary>
        public async Task InitChatClient()
        {                        
            try
            {                
                //qs Create the chat client
                client = new ChatClient(username);
                // add an event handler for incoming messages
                client.MessageReceived += MessageReceived;
                // start the client
                Console.WriteLine($"Index: chart starting...");
                await client.Start();
                Console.WriteLine($"Index: chart started?");
            }
            catch (Exception e)
            {
                var msg = $"ERROR: Failed to start chat client: {e.Message}";
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Inbound message from Hub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"Blazor: received {e.Message}");
            IsRocketLaunchRunning = e.Message == "RlStarted" ? true : false;
            OnStateChanged?.Invoke();
        }

        async Task SendMessage(string message)
        {
            await client.Send(message);
        }

        private bool disposed = false;        
        public void Dispose()
        {
            Console.WriteLine($"App state disposing. Disposed: {disposed}");
            if (!disposed)
            {
                disposed = true;
            }
        }
        #endregion
    }

    class Message
    {
        public Message(string username, string body, bool mine)
        {
            Username = username;
            Body = body;
            Mine = mine;
        }

        public string Username { get; set; }
        public string Body { get; set; }
        public bool Mine { get; set; }

        /// <summary>
        /// Determine CSS classes to use for message div
        /// </summary>
        public string CSS
        {
            get
            {
                return Mine ? "sent" : "received";
            }
        }

    }
}
