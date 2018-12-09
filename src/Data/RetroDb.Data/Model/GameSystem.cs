using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroDb.Data
{
    public class GameSystem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public bool Enabled { get; set; }        

        public DateTime? LastLaunched { get; set; }

        //public string? Image { get; set; }

        public GameSystemType SystemType { get; set; }

        public ICollection<Emulator> Emulators { get; set; }

    }
}
