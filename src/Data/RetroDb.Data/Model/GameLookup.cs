namespace RetroDb.Data.Model
{
    public class GameLookup
    {
        public int Id { get; set; }
        public string ShortDescription { get; set; }
        public bool Favorite { get; set; }
        public string Genre { get; set; }
        public string Manufacturer { get; set; }        
        public int Year { get; set; }
    }
}
