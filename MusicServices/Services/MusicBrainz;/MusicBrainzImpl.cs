using System;
using System.Threading.Tasks;
using Grpc.Core;
using MusicBrainz;

namespace MusicServices.Services.MusicBrainz
{
    public class MusicBrainzImpl : MusicBrainzService.MusicBrainzServiceBase
    {
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
