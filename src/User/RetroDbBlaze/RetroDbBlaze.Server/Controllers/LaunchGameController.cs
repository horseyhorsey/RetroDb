using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RetroDb.Api.Hubs;
using RetroDb.Data;
using RetroDb.Engine.Frontends;
using RetroDb.Repo;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RetroDbBlaze.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LaunchGameController : RetroDbControllerBase
    {
        private static bool gameRunning = false;
        private static Game _runningGame = null;
        
        private IHubContext<RocketLauncherHub> _hubContext;
        private RocketLauncher _rocketLauncher;

        public LaunchGameController(IHubContext<RocketLauncherHub> hubContext, IUnitOfWork unitOfWork, RocketLauncher rocketLauncher) : base(unitOfWork)
        {
            _hubContext = hubContext;
            _rocketLauncher = rocketLauncher;
        }

        [Route("Running")]
        [HttpGet]
        public Task<bool> IsRocketLauncherRunning()
        {
            bool result = IsRocketLaunchProcessRunning();
            return Task.FromResult(result);
        }

        private static bool IsRocketLaunchProcessRunning()
        {
            Process[] rocky = GetRlProcess();
            bool result = rocky?.Length > 0 ? true : false;
            return result;
        }

        private static Process[] GetRlProcess()
        {
            return Process.GetProcessesByName("RocketLauncher");
        }

        [Route("Pause/Nav")]
        [HttpPost("{commandName}")]
        public Task<int> NavigatePause([FromBody] string commandName)
        {
            return _rocketLauncher.NavigatePause(commandName);
        }

        [Route("Pause")]
        [HttpGet]
        public Task<int> PauseGame()
        {
            return _rocketLauncher.Pause();
        }

        [Route("Quit")]
        [HttpGet]
        public Task<int> QuitGame()
        {
            return _rocketLauncher.Quit();
        }

        [Route("RunningGame")]
        [HttpGet]
        public Task<Game> RunningGame()
        {           
            return Task.FromResult(_runningGame);
        }

        [HttpGet("{id}")]
        public async Task Launch(int id)
        {
            if (gameRunning) await Task.FromResult(false);            

            await Task.Run(async () =>
            {
                bool errored = false;
                var game = await _unitOfWork.GamesRepository.GetByIDAsync(id);                
                try
                {
                    var system = await _unitOfWork.GamingSystemRepository.GetByIDAsync(game.SystemId);
                    gameRunning = true;                    
                    if (game != null)
                    {
                        _runningGame = game;
                        RocketLauncher rl = new RocketLauncher("I:\\Rocketlauncher");
                        var startTime = DateTime.Now;
                        system.LastLaunched = startTime;                        
                        game.LastPlayed = startTime;

                        await _hubContext.Clients.All.SendAsync("ReceiveMessage", "RetroDb", "RlStarted");
                        rl.Launch(game.FileName, system.Name);

                        var endTime = DateTime.Now;
                        game.TimePlayed = endTime - startTime;                        
                        //Todo: add times played
                    }

                }
                catch
                {
                    errored = true;
                }
                finally
                {
                    gameRunning = false;
                    _runningGame = null;
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", "RetroDb", "RlEnded");
                }

                if (!errored)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", "RetroDb", "RlError");
                    _unitOfWork.GamesRepository.Update(game);
                    _unitOfWork.Save();
                }
                    
            });            
        }
    }
}