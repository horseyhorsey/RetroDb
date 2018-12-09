using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RetroDb.Data;
using RetroDb.DataSqlite;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RetroDb.Repo
{
    public interface IUnitOfWork
    {
        void Dispose();
        BaseRepo<GameSystem> GamingSystemRepository { get; }
        BaseRepo<Game> GamesRepository { get; }
        BaseRepo<Genre> GenreRepository { get; }
        void Save();
        Task SaveAsync();
    }

    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Fields
        private RetroDbContext _ctx;
        private string _constring;
        private bool disposed = false;
        #endregion

        #region Constructors
        public UnitOfWork(string connstring)
        {
            _ctx = new RetroDbContext(connstring);
            _constring = connstring;
        } 
        #endregion

        #region Repos
        private BaseRepo<Emulator> _emulatorsRepository;
        private BaseRepo<Game> _gamesRepo;
        private BaseRepo<GameSystem> _gamingSystemRepository;
        private BaseRepo<Genre> _genreRepository;
        private BaseRepo<HiScore> _hiScoreRepository;
        private BaseRepo<Manufacturer> _manufacturerRepo;
        private BaseRepo<Publisher> _publisherRepository;
        private BaseRepo<Tool> _toolsRepository;

        public BaseRepo<Emulator> EmulatorsRepository
        {
            get
            {
                if (_emulatorsRepository == null)
                {
                    _emulatorsRepository = new BaseRepo<Emulator>(_ctx);
                }
                return _emulatorsRepository;
            }
            set { _emulatorsRepository = value; }
        }
        public BaseRepo<Game> GamesRepository
        {
            get
            {
                if (_gamesRepo == null)
                {
                    _gamesRepo = new BaseRepo<Game>(_ctx);
                }
                return _gamesRepo;
            }
            set { _gamesRepo = value; }
        }
        public BaseRepo<GameSystem> GamingSystemRepository
        {
            get
            {
                if (_gamingSystemRepository == null)
                {
                    _gamingSystemRepository = new BaseRepo<GameSystem>(_ctx);
                }
                return _gamingSystemRepository;
            }
            set { _gamingSystemRepository = value; }
        }
        public BaseRepo<Genre> GenreRepository
        {
            get
            {
                if (_genreRepository == null)
                {
                    _genreRepository = new BaseRepo<Genre>(_ctx);
                }
                return _genreRepository;
            }
            set { _genreRepository = value; }
        }
        public BaseRepo<HiScore> HiScoreRepository
        {
            get
            {
                if (_hiScoreRepository == null)
                {
                    _hiScoreRepository = new BaseRepo<HiScore>(_ctx);
                }
                return _hiScoreRepository;
            }
            set { _hiScoreRepository = value; }
        }
        public BaseRepo<Manufacturer> ManufacturerRepository
        {
            get
            {
                if (_manufacturerRepo == null)
                {
                    _manufacturerRepo = new BaseRepo<Manufacturer>(_ctx);
                }
                return _manufacturerRepo;
            }
            set { _manufacturerRepo = value; }
        }
        public BaseRepo<Publisher> PublisherRepository
        {
            get
            {
                if (_publisherRepository == null)
                {
                    _publisherRepository = new BaseRepo<Publisher>(_ctx);
                }
                return _publisherRepository;
            }
            set { _publisherRepository = value; }
        }
        public BaseRepo<Tool> ToolsRepository
        {
            get
            {
                if (_toolsRepository == null)
                {
                    _toolsRepository = new BaseRepo<Tool>(_ctx);
                }
                return _toolsRepository;
            }
            set { _toolsRepository = value; }
        }
        #endregion

        #region Public Methods
        public bool EnsureCreated() => _ctx.Database.EnsureCreated();
        public void Save() => _ctx.SaveChanges();
        public Task SaveAsync() => _ctx.SaveChangesAsync();
        #endregion

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _ctx.Dispose();
                }
            }
            this.disposed = true;
        }

        internal void DetatchTrackedEntites()
        {
            foreach (EntityEntry entityEntry in _ctx.ChangeTracker.Entries().ToArray())
            {
                if (entityEntry.Entity != null)
                {
                    entityEntry.State = EntityState.Detached;
                }
            }
        }
        #endregion
    }
}
