using System;
using System.Collections.Generic;
using System.Text;

namespace MusicServices.Database
{
    public class StoredLyric
    {
        public string ArtistName { get; set; }
        public string SongTitle { get; set; }
        public string LyricText { get; set; }
        public int LyricWordsCount { get; set; }
    }
}
