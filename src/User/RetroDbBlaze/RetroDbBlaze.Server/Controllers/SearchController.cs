using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RetroDb.Data;

using RetroDb.Repo;

namespace RetroDbBlaze.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : RetroDbControllerBase
    {
        public SearchController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        public PagedResult<Game> Search(string search, int index = 0, int pageSize = 20)
        {
            var query = _unitOfWork.GamesRepository
            .GetQuery(
            x => x.ShortDescription.ToLower().Contains(search.ToLower()),
            orderBy: x => x.OrderBy(z => z.ShortDescription),
            includeProperties: "System"
            );

            var count = query.Count();
            var games = query.Skip(index * pageSize).Take(pageSize).AsEnumerable();

            return new PagedResult<Game>(games, index, pageSize, count);
        }

        [HttpPost]
        public PagedResult<Game> SearchGames([FromBody]GameSearchOption gameSearchOption)
        {
            var query = _unitOfWork.GamesRepository
            .GetQuery(
            x => x.ShortDescription.ToLower().Contains(gameSearchOption.SearchText.ToLower()),
            orderBy: x => x.OrderBy(z => z.ShortDescription),
            includeProperties: "System"
            );

            int psize = 20, pNumber = gameSearchOption.PageNumber;
            if (gameSearchOption.PageSize.HasValue) psize = gameSearchOption.PageSize.Value;            

            var count = query.Count();
            var games = query.Skip((pNumber-1) * psize).Take(psize).AsEnumerable();

            return new PagedResult<Game>(games, pNumber, psize, count);
        }
    }
}