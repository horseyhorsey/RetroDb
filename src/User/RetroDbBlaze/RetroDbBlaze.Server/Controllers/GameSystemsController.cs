using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RetroDb.Data;
using RetroDb.Data.Model;
using RetroDb.Repo;

namespace RetroDbBlaze.Server.Controllers
{
    [Route("api/systems")]
    [ApiController]
    public class GameSystemsController : RetroDbControllerBase
    {
        public GameSystemsController(IUnitOfWork unitOfWork):base(unitOfWork)
        {            
        }

        // GET: api/GameSystems
        [HttpGet]
        public Task<List<GameSystem>> Get()
        {
            return _unitOfWork.GamingSystemRepository.GetAsync();
        }

        [HttpGet("{sysId}/Games")]
        public IEnumerable<object> GetGames(int sysId)
        {
            return _unitOfWork.GamesRepository.Get(
                x => x.SystemId == sysId)
                .Select(z => new { Id = z.Id, FileName = z.FileName });
        }

        //// GET: api/GameSystems/5
        [HttpGet("{id}", Name = "Get")]
        public Task<GameSystem> Get(int id)
        {
            return _unitOfWork.GamingSystemRepository.GetByIDAsync(id);
        }

        [HttpGet("{id}/Info", Name = "GetInfo")]
        public async Task<GameSystemInfo> GetSystemInfo(int id)
        {
            var system = await _unitOfWork.GamingSystemRepository.GetByIDAsync(id);

            if (system == null)
                return null;

            var systemGames = _unitOfWork.GamesRepository.GetQuery(x => x.SystemId == id);
            var sysInfo = new GameSystemInfo(system);
            var timePlayed = systemGames.Where(x => x.TimePlayed.HasValue).Sum(x => x.TimePlayed.Value.TotalSeconds);

            //Counts and time
            sysInfo.FavoriteCount = systemGames.Where(x => x.Favourite).Count();
            sysInfo.Timeplayed = (int)timePlayed;
            sysInfo.GameCount = systemGames.Count();            

            //Add last played games list
            sysInfo.LastPlayedGames = systemGames.Where(x => x.LastPlayed.HasValue)?
                .Select(x => new Game()
                {
                    Id = x.Id,
                    FileName = x.FileName,
                    ShortDescription = x.ShortDescription,
                    LastPlayed = x.LastPlayed,
                    TimePlayed = x.TimePlayed,
                    System = system
                }).OrderByDescending(x => x.LastPlayed).Take(5).ToList();

            return sysInfo;
        }        

        [HttpGet("{id}/Favorites")]
        public async Task<IEnumerable<Game>> GetFavorites(int id)
        {
            var system = await _unitOfWork.GamingSystemRepository.GetByIDAsync(id);
            if (system == null)
                return null;

            return _unitOfWork.GamesRepository.GetQuery(x => x.SystemId == id && x.Favourite).AsEnumerable();
        }

        //// POST: api/GameSystems
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT: api/GameSystems/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
