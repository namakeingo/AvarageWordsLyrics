using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MusicServices.DataContracts.MusicBrainz
{
    class MusicBrainz_Type
    {
        [JsonProperty("count")]
        public int count { get; set; }

        [JsonProperty("offset")]
        public int offset { get; set; }

        [JsonProperty("artists")]
        public MusicBrainz_Artist_Type[] artists { get; set; }

        [JsonProperty("recordings")]
        public MusicBrainz_Recording_Type[] recordings { get; set; }
    }

    class MusicBrainz_Artist_Type
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("type")]
        public MusucBrainz_ArtistType_Enum type { get; set; }

        [JsonProperty("score")]
        public int score { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("gender")]
        public string gender { get; set; }

        [JsonProperty("country")]
        public string country { get; set; }

        [JsonProperty("disambiguation")]
        public string disambiguation { get; set; }

        //This definition is not complete but we don't need the other parameters of the artist object
    }

    [JsonConverter(typeof(StringEnumConverter))]
    enum MusucBrainz_ArtistType_Enum
    {
        Person,
        Group,
        Orchestra,
        Choir,
        Character,
        Other
    }

    class MusicBrainz_Recording_Type
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("score")]
        public int score { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("length")]
        public int length { get; set; }

        //This definition is not complete but we don't need the other parameters of the recording object
    }
}
