# Install packages for Sqlite

	dotnet add package Microsoft.EntityFrameworkCore.Sqlite
	dotnet add package Microsoft.EntityFrameworkCore.Design

# Create models with data annotations and context

## See On model creating to add Unique keys

        public DbSet<Game> Games { get; set; }
        public DbSet<GamingSystem> GamingSystems { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Retro.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //assign unique id for name of system
            modelBuilder.Entity<GamingSystem>(e =>
            {
                e.HasIndex(f => f.Name).IsUnique();
            });

            modelBuilder.Entity<Game>(g =>
            {
                g.HasIndex(f => new { f.Title, f.SystemId, f.ManufacturerId })                
                .IsUnique();
            });
        }

# Create a migration

	dotnet ef add migration InitialCreate

# Update Database

	dotnet ef database update
