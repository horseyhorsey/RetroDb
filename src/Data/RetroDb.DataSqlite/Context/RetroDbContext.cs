using Microsoft.EntityFrameworkCore;
using RetroDb.Data;
using System;
using System.IO;

namespace RetroDb.DataSqlite
{
    public class RetroDbContext : DbContext
    {
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Emulator> Emulators { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<GameSystem> GamingSystems { get; set; }
        public DbSet<HiScore> HiScores { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Tool> Tools { get; set; }

        private string _conString = string.Empty;

        public RetroDbContext(): this (@"Data Source=C:\ProgramData\RetroDb\RetroDb.Db")
        {        
            
        }

        public RetroDbContext(string connectionstring)
        {
            if (string.IsNullOrWhiteSpace(_conString))
                _conString = connectionstring;

            if (!File.Exists(connectionstring.Replace("Data Source=", string.Empty)))
                this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_conString);            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Emulator>(e =>
            {
                e.HasIndex(f => new { f.Name , f.Version, f.Executable}).IsUnique();
            });

            modelBuilder.Entity<Developer>(e =>
            {
                e.HasIndex(f => new { f.Name }).IsUnique();
            });

            modelBuilder.Entity<Publisher>(e =>
            {
                e.HasIndex(f => new { f.Name }).IsUnique();
            });

            modelBuilder.Entity<Genre>(e =>
            {
                e.HasIndex(f => f.Name).IsUnique();
            });

            //assign unique id for name of system
            modelBuilder.Entity<GameSystem>(e =>
            {
                e.HasIndex(f => f.Name).IsUnique();
            });

            modelBuilder.Entity<Game>(g =>
            {
                g.HasIndex(f => new { f.Title, f.FileName, f.Description, f.SystemId, f.ManufacturerId, f.GenreId, f.Year})                
                .IsUnique();
            });

            modelBuilder.Entity<Manufacturer>(e =>
            {
                e.HasIndex(f => f.Name).IsUnique();
            });

            modelBuilder.Entity<Tool>(e =>
            {
                e.HasIndex(f => new { f.Name}).IsUnique();
            });
            
        }
    }
}
