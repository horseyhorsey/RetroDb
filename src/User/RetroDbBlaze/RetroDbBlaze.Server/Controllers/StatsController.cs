using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetroDb.Data;
using RetroDb.Repo;

namespace RetroDbBlaze.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : RetroDbControllerBase
    {
        public StatsController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        public Task<RetroDbStats> Get()
        {
            var retroDbStats = new RetroDbStats();

            var recentPlayed =_unitOfWork.GamesRepository.GetQuery(orderBy: x => x.OrderByDescending(z => z.LastPlayed), includeProperties: "System").Take(10);
            var mostPlayed   = _unitOfWork.GamesRepository.GetQuery(orderBy: x => x.OrderByDescending(z => z.TimePlayed), includeProperties: "System").Take(10);
            retroDbStats.RecentPlayed = recentPlayed.ToArray();
            retroDbStats.MostPlayed = mostPlayed.ToArray();

            return Task.FromResult(retroDbStats);
        }
    }
}