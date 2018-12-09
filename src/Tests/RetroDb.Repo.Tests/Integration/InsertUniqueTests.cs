using RetroDb.Data;
using RetroDb.DataSqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetroDb.Repo.Tests.Integration
{
    public class InsertUniqueTests
    {
        private RetroDbContext _ctx;

        Manufacturer manu = new Data.Manufacturer { Name = "Nintendo" };
        GameSystem gs = new Data.GameSystem { Name = "Nintendo" };

        string conString = Constants.CONN_STRING;

        private static bool _created = false;

        public InsertUniqueTests()
        {
            if (!_created)
            {
                using (_ctx = new RetroDbContext(conString))
                {
                    _ctx.Database.EnsureCreated();
                    _created = true;
                }
            }
        }

        /// <summary>
        /// Remove GenreId 75 to 39 and update the two existing games
        /// </summary>
        /// <returns></returns>
        [Theory(Skip = "Integration")]   
        [InlineData(75, 39)]
        public async Task UpdateGenreAndGames(int removeId, int moveToId)
        {
            using (_ctx = new RetroDbContext(conString))
            {
                var transaction = await _ctx.Database.BeginTransactionAsync();
                try
                {
                    //games to change
                    var games = _ctx.Games.Where(x => x.GenreId == removeId);
                    foreach (var game in games)
                    {
                        game.GenreId = moveToId;
                    }
                    
                    //Genre to keep
                    var genre = _ctx.Genres.FirstOrDefault(x => x.Id == removeId);
                    if (genre != null)
                        _ctx.Genres.Remove(genre);

                    _ctx.UpdateRange(games);
                    await _ctx.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }

            }
        }

    }
}