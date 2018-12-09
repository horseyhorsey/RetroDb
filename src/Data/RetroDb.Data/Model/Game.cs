using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroDb.Data
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public bool Favourite { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }        
        public string FileName { get; set; }
        public int? Players { get; set; }
        
        public int SystemId { get; set; }                        
        public string Region { get; set; }
        public string Rating { get; set; }
        public int? Year { get; set; }

        public int? DeveloperId { get; set; }
        public int? EmulatorId { get; set; }
        public int? GenreId { get; set; }        
        public int? ManufacturerId { get; set; }

        public int? TimesPlayed { get; set; }
        public DateTime? LastPlayed { get; set; }
        public DateTime? LastUpdated { get; set; }
        public TimeSpan? TimePlayed { get; set; }        

        /// <summary>
        /// Mark a game to play later.
        /// </summary>
        public bool? PlayLater { get; set; }

        [MinLength(0)]
        [MaxLength(10)]
        public int UserRating { get; set; }
                
        public GameSystem System { get; set; }
        public Genre Genre { get; set; }
        public Emulator Emulator { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public Developer Developer { get; set; }

        public GameControlType GameControlType { get; set; }
    }

    public class GameRunning
    {
        public Game Game { get; set; }

        public GameRunning(Game game)
        {
            Game = game;    
        }
    }
}
