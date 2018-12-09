using Microsoft.EntityFrameworkCore;
using RetroDb.Data;
using RetroDb.Data.Frontend.RocketLauncher;
using RetroDb.Engine.Frontends;
using RetroDb.Engine.Import;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RetroDb.Repo
{
    public interface IBulkImport
    {
        IFrontEnd FrontEnd { get; }
        Task ImportAsync();
        event EventLog ImportProgressChanged;
    }

    public delegate void EventLog(string msg);

    public class BulkImport : IBulkImport, IDisposable
    {
        public IFrontEnd FrontEnd { get; }
        private UnitOfWork _uow;
        private RocketLauncher _rocketLauncher;

        public event EventLog ImportProgressChanged;

        #region Constructors
        public BulkImport(string connstring, string feType, string fePath, string rlPath)
        {
            var builder = new FrontEndBuilder();
            FrontEnd = builder.GetFrontEnd(feType, fePath);
            _uow = new UnitOfWork(connstring);
            _rocketLauncher = new RocketLauncher(rlPath);
        }
        #endregion

        #region Public methods

        public async Task ImportAsync()
        {
            var sw = new Stopwatch();
            try
            {
                sw.Start();
                ImportProgressChanged?.Invoke($"Starting Bulk Frontend import for frontend path: {FrontEnd.FePath}");
                //_uow.EnsureCreated(); //Don't need this run migrate instead in context

                ImportProgressChanged?.Invoke("populating systems");
                IEnumerable<GameSystem> systems = await ImportSystemsAsync();
                ImportProgressChanged?.Invoke("populated systems");

                int sysImportCount = 0; int failedSysImportCount = 0;
                int gameImportCount = 0; int failedGameImportCount = 0;
                foreach (var system in systems)
                {
                    try
                    {
                        ImportProgressChanged?.Invoke($"populatingsystem: {system.Name}");

                        //Get an existing system or insert one
                        var existingSystemId = (await _uow.GamingSystemRepository
                            .GetAsync(x => x.Name == system.Name)).FirstOrDefault()?.Id;

                        //Get games for this system
                        IEnumerable<Game> games = null;
                        ImportProgressChanged?.Invoke($"populating games for: {system.Name}");
                        try { games = await FrontEnd.GetGamesAsync(system.Name); }
                        catch (Exception ex)
                        {
                            ImportProgressChanged?.Invoke($"no games / database exists for: {system.Name}. {ex.Message}");
                            failedGameImportCount++;
                            continue;
                        }

                        //Favorites
                        IEnumerable<string> favorites = null;
                        try { favorites = await FrontEnd.GetFavoritesAsync(system.Name); }
                        catch(Exception ex) { ImportProgressChanged?.Invoke($"Favorites Error: {ex.Message}"); };

                        //Rocketlauncher Stats
                        IEnumerable<Stat> systemGameStats = null;
                        try { systemGameStats = await _rocketLauncher.GetStatsAsync(system.Name); }
                        catch (Exception ex) { ImportProgressChanged?.Invoke($"Stats Error: {ex.Message}"); }

                        ImportProgressChanged?.Invoke($"inserting games: {system.Name}");
                        int gameCount = 0;
                        int i = 0;

                        //await ImportGenresAsync(games);
                        //await ImportManufacturersAsync(games);

                        foreach (var game in games)
                        {
                            if (existingSystemId != null)
                            {
                                game.SystemId = existingSystemId.HasValue ? existingSystemId.Value : 0;
                                game.System = null;
                            }                            

                            //Add or find genre
                            int? genreId = (await _uow.GenreRepository.GetAsync(x => x.Name == game.Genre.Name)).FirstOrDefault()?.Id;
                            if (genreId.HasValue)
                            {
                                game.GenreId = genreId.Value;
                                game.Genre = null;
                            }

                            //Add or find manufacturer
                            int? manuId = (await _uow.ManufacturerRepository.GetAsync(x => x.Name == game.Manufacturer.Name)).FirstOrDefault()?.Id;
                            if (manuId.HasValue)
                            {
                                game.ManufacturerId = manuId.Value;
                                game.Manufacturer = null;
                            }
                            
                            //Is Favorite?
                            game.Favourite = !string.IsNullOrWhiteSpace(favorites?.FirstOrDefault(x => x == game.FileName));

                            //Has RL stats
                            var rlGameStat = systemGameStats?.FirstOrDefault(x => x.Rom == game.FileName);
                            if (rlGameStat != null)
                            {
                                game.LastPlayed = rlGameStat.LastTimePlayed;                                
                                game.TimesPlayed = rlGameStat.TimesPlayed;
                                game.TimePlayed = rlGameStat.TotalTimePlayed;
                                ImportProgressChanged?.Invoke($"Added stats from Rockelauncher: {game.FileName}");
                            }                            

                            //Insert game
                            ImportProgressChanged?.Invoke($"inserting game: {game.FileName} - {game.Description}, is favorite {game.Favourite}");
                            await _uow.GamesRepository.InsertAsync(game);

                            gameCount++;
                            i++;

                            try
                            {
                                await _uow.SaveAsync();
                                gameImportCount++;
                            }
                            catch (DbUpdateException dbupdateEx)
                            {
                                if (dbupdateEx.Entries?.Count > 0)
                                {
                                    var errGame = dbupdateEx.Entries?[0].Entity as Game;
                                    ImportProgressChanged?.Invoke($"Exception: {errGame?.FileName} {errGame?.System.Name}");
                                }
                                _uow.GamesRepository.Delete(game);
                                ImportProgressChanged?.Invoke($"{dbupdateEx.Message}");
                                ImportProgressChanged?.Invoke($"{dbupdateEx.InnerException?.Message}");
                            }

                        }

                        //await _uow.SaveAsync();
                        i = 0;
                        sysImportCount++;
                        ImportProgressChanged?.Invoke($"System imported: {gameCount} entries");
                        _uow.DetatchTrackedEntites();
                    }
                    catch (Exception ex)
                    {
                        failedSysImportCount++;
                        ImportProgressChanged?.Invoke($"{ex.Message}");
                        continue;
                    }
                }

                ImportProgressChanged?.Invoke($"import completed: Systems / Failed: {sysImportCount}:{failedSysImportCount}, " +
                    $"Games / Failed: {gameImportCount} : {failedGameImportCount}");
            }
            catch (Exception ex)
            {
                ImportProgressChanged?.Invoke($"{ex.Message}");
                ImportProgressChanged?.Invoke($"{ex.InnerException?.Message}");
                throw;
            }
            finally
            {
                sw.Stop();
                ImportProgressChanged?.Invoke($"Completed Time: {sw.Elapsed.ToString()}");
            }
        }

        private async Task<IEnumerable<GameSystem>> ImportSystemsAsync()
        {
            var systems = await FrontEnd.GetSystemsAsync();
            foreach (var system in systems)
            {
                try
                {
                    var sysObj = (await _uow.GamingSystemRepository.GetAsync(x => x.Name == system.Name)).FirstOrDefault();
                    if (sysObj == null)
                    {
                        await _uow.GamingSystemRepository.InsertAsync(system);
                        await _uow.SaveAsync();
                    }
                    else
                    {
                        system.Id = sysObj.Id;
                    }
                }
                catch (Exception ex) { ImportProgressChanged?.Invoke($"failed to save system to db: {ex.Message}"); }
            }

            return systems;
        }

        private async Task ImportGenresAsync(IEnumerable<Game> games)
        {
            var genres = games.Select(x => x.Genre.Name).Distinct();
            foreach (var genre in genres)
            {
                try
                {
                    var genreObj = (await _uow.GenreRepository.GetAsync(x => x.Name == genre)).FirstOrDefault();
                    if (genreObj == null)
                    {
                        await _uow.GenreRepository.InsertAsync(new Genre { Name = genre });
                        await _uow.SaveAsync();
                    }                    
                }
                catch (Exception ex) { ImportProgressChanged?.Invoke($"failed to save genre to db: {ex.Message}"); }
            }
        }

        private async Task ImportManufacturersAsync(IEnumerable<Game> games)
        {
            var manufacturers = games.Select(x => x.Manufacturer.Name).Distinct();
            foreach (var manu in manufacturers)
            {
                try
                {
                    var manuObj = (await _uow.ManufacturerRepository.GetAsync(x => x.Name == manu)).FirstOrDefault();
                    if (manuObj == null)
                    {
                        await _uow.ManufacturerRepository.InsertAsync(new Manufacturer { Name = manu });
                        await _uow.SaveAsync();
                    }
                }
                catch (Exception ex) { ImportProgressChanged?.Invoke($"failed to save manufacturer to db: {ex.Message}"); }
            }
        }

        #endregion

        #region Dispose
        private bool _isDisposed = false;
        protected virtual void Dispose(bool isDisposing)
        {
            if (!_isDisposed)
            {
                if (isDisposing)
                {
                    _uow.Dispose();
                }
            }
            this._isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
