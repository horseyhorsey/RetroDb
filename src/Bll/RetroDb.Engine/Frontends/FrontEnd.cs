using RetroDb.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetroDb.Engine
{
    /// <summary>
    /// Represents an external game frontend
    /// </summary>
    internal abstract class FrontEnd : IFrontEnd
    {
        public string FePath { get; }

        public FrontEnd(string feDirectory)
        {
            FePath = feDirectory;
        }

        public virtual Task<IEnumerable<string>> GetFavoritesAsync(string systemName)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<Game>> GetGamesAsync(string systemName, string name = null, bool mainMenuDb = false)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<GameSystem>> GetSystemsAsync(string dbName = null)
        {
            throw new NotImplementedException();
        }

    }
}
