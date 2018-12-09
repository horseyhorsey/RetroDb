using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RetroDb.Data
{
    public class HiScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
        public long Score { get; set; }
        public DateTime CreatedDate { get; set; }
        public Game Game { get; set; }        
        public Player Player { get; set; }
    }
}
