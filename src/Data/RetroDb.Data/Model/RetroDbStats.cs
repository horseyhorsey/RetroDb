using System;
using System.Collections.Generic;
using System.Text;

namespace RetroDb.Data
{
    public class RetroDbStats
    {
        public Game[] RecentPlayed { get; set; }
        public Game[] MostPlayed { get; set; }
    }
}
