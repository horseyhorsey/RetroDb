using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroDb.Data
{
    public class Emulator
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Executable { get; set; }

        public string GamePaths { get; set; }

        public string Extensions { get; set; }

        public string Version { get; set; }

        public int? GamingSystemId { get; set; }

        public GameSystem GamingSystem { get; set; }
    }
}
