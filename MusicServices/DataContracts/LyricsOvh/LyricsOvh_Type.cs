using Newtonsoft.Json;

namespace MusicServices.DataContracts.LyricsOvh
{
    class LyricsOvh_Type
    {
        [JsonProperty("lyrics")]
        public string lyrics { get; set; }
    }
}
