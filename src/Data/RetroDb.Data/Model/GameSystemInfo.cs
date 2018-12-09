using System.Collections.Generic;

namespace RetroDb.Data.Model
{
    public class GameSystemInfo
    {
        public GameSystemInfo()
        {

        }

        public GameSystemInfo(GameSystem gameSystem)
        {
            GameSystem = gameSystem;
        }

        public GameSystem GameSystem { get; private set; }

        public int FavoriteCount { get; set; }

        public int GameCount { get; set; }

        public int Timeplayed { get; set; }

        public List<Game> LastPlayedGames { get; set; }
    }
}
