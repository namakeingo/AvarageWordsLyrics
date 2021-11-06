using System.Runtime.Serialization;

namespace MusicServices.DataContracts.LyricsOvh
{
    [DataContract]
    class LyricsOvh_Type
    {
        [DataMember(Name = "lyrics")]
        public string lyrics { get; set; }
    }
}
