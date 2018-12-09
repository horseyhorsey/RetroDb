using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RetroDb.Data;
using RetroDb.Data.Model;
using RetroDb.Repo;

namespace RetroDbBlaze.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : RetroDbControllerBase
    {
        public GamesController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        // GET: api/Games
        [HttpGet("BySystem/{sysId}")]
        public IEnumerable<Game> GetGames(int sysId)
        {
            return _unitOfWork.GamesRepository.Get(
                x => x.SystemId == sysId,
                orderBy: x => x.OrderBy(z => z.FileName),
                includeProperties:"System");
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = (await _unitOfWork.GamesRepository.GetAsync(x=> x.Id == id, includeProperties:"System")).FirstOrDefault();
            if (game == null)
            {
                return NotFound();
            }

            return Ok(game);
        }

        [HttpGet("Favorites")]
        public PagedResult<Game> GetFavorites(int page = 1, int pageSize = 10)
        {
            if (page <= 0)
                page = 1;

            var faveGames = _unitOfWork.GamesRepository.GetQuery(x => x.Favourite, includeProperties:"System", orderBy: x => x.OrderBy(z => z.ShortDescription));
            var count = faveGames.Count();
            var games = faveGames.Skip(page * pageSize).Take(pageSize).AsEnumerable();

            return new PagedResult<Game>(games, page, pageSize, count);
        }

        [HttpGet("Lookup/{systemId}")]
        public IEnumerable<GameLookup> GetGameLookUp(int systemId)
        {
            return _unitOfWork.GamesRepository.Get(x => x.SystemId == systemId, includeProperties:"Genre,Manufacturer")
                .Select(x => new GameLookup
                {
                    Genre = x.Genre.Name,
                    Manufacturer = x.Manufacturer.Name,
                    ShortDescription = x.ShortDescription,
                    Year = (int)x.Year,
                    Favorite = x.Favourite,
                    Id = x.Id
                });
        }

        // PUT: api/Games/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutGame([FromRoute] int id, [FromBody] Game game)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != game.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(game).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!GameExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Games
        //[HttpPost]
        //public async Task<IActionResult> PostGame([FromBody] Game game)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Games.Add(game);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetGame", new { id = game.Id }, game);
        //}

        //// DELETE: api/Games/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteGame([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var game = await _context.Games.FindAsync(id);
        //    if (game == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Games.Remove(game);
        //    await _context.SaveChangesAsync();

        //    return Ok(game);
        //}

        private bool GameExists(int id)
        {
            return _unitOfWork.GamesRepository.GetByID(id) == null ? false : true;
        }
    }
}