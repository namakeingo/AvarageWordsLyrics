using System;
using System.Threading.Tasks;
using Grpc.Core;

using MusicBrainz;

namespace MusicServices.Services.MusicBrainz
{
    public class MusicBrainzService : MusicBrainzProto.MusicBrainzProtoBase
    {
        const string API_BASE_URL = "http://musicbrainz.org/ws/2/";
        const string ARTIST_URL = API_BASE_URL + "artist/?fmt=json&query=";
        const string SONGS_URL = API_BASE_URL + "recordings/?fmt=json&query=";

        // Server Side handler of the GetLyrics RPC
        public override Task<MusicBrainz_SearchArtist_Reply> SearchArtist(MusicBrainz_SearchArtist_Request request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
        // Server Side handler of the GetLyrics RPC
        public override Task<MusicBrainz_SearchArtistSongs_Reply> SearchArtistSongs(MusicBrainz_SearchArtistSongs_Request request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
