using System.Runtime.Serialization;

namespace MusicServices.DataContracts.MusicBrainz
{
    [DataContract]
    class MusicBrainz_Type
    {
        [DataMember(Name = "count")]
        public int count { get; set; }

        [DataMember(Name = "offset")]
        public int offset { get; set; }

        [DataMember(Name = "artists")]
        public MusicBrainz_Artist_Type[] artists { get; set; }

        [DataMember(Name = "recordings")]
        public MusicBrainz_Recording_Type[] recordings { get; set; }
    }

    [DataContract]
    class MusicBrainz_Artist_Type
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        //This definition is not complete but we don't need the other parameters of the artist object
    }

    [DataContract]
    class MusicBrainz_Recording_Type
    {
        [DataMember(Name = "id")]
        public string id { get; set; }


        [DataMember(Name = "title")]
        public string title { get; set; }

        //This definition is not complete but we don't need the other parameters of the recording object
    }
}
