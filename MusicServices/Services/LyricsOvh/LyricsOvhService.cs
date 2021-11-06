using System;
using System.Threading.Tasks;
using Grpc.Core;

using LyricsOvh;

namespace MusicServices.Services.LyricsOvh
{
    public class LyricsOvhService : LyricsOvhProto.LyricsOvhProtoBase
    {
        const string API_BASE_URL = "https://api.lyrics.ovh/v1/{0}/{1}";

        // Server Side handler of the GetLyrics RPC
        public override Task<LyricsOvh_Reply> GetLyric(LyricsOvh_Request request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        // Server Side handler of the GetLyricWithDatabaseHelp RPC
        public override Task<LyricsOvh_Reply> GetLyricWithDatabaseHelp(LyricsOvh_Request request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

    }
}
