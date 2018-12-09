using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetroDb.Data
{
    /// <summary>
    /// Represents an external frontend
    /// </summary>
    public interface IFrontEnd
    {
        /// <summary>
        /// Gets the Frontend path
        /// </summary>
        string FePath { get; }

        /// <summary>
        /// Gets games for a given system
        /// </summary>
        /// <param name="systemName"></param>
        /// <param name="name">The name of the Db. If null will use the systems name</param>
        /// <param name="mainMenuDb">Is this a main menu type</param>
        /// <returns></returns>
        Task<IEnumerable<Game>> GetGamesAsync(string systemName, string name = null, bool mainMenuDb = false);

        /// <summary>
        /// Gets Favorites for a system. (hyperspin only)
        /// </summary>
        /// <param name="systemName">The system name</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetFavoritesAsync(string systemName);

        /// <summary>
        /// Gets the systems set in this frontend
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        Task<IEnumerable<GameSystem>> GetSystemsAsync(string dbName = null);
    }
}
