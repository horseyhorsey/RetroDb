namespace RetroDb.Data
{
    public class GameSearchOption
    {
        public GameControlType GameControlType { get; set; }
        public GameSystemType GameSystemType { get; set; }
        public int[] SystemIds { get; set; }        
        public string SearchText { get; set; }
        public int PageNumber { get; set; }
        public int? PageSize { get; set; }

        public GameSearchOption(string search, int pageNumber, int? pageSize)
        {
            SearchText = search;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public GameSearchOption()
        {

        }
    }
}
