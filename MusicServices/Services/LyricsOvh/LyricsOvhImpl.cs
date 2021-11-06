using System;
using System.Threading.Tasks;
using Grpc.Core;
using LyricsOvh;

namespace MusicServices.Services.LyricsOvh
{
    class LyricsOvhImpl : LyricsOvhService.LyricsOvhServiceBase
    {
        // Server Side handler of the GetLyrics RPC
        public override Task<LyricsOvh_Reply> GetLyric(LyricsOvh_Request request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        // Server Side handler of the GetLyrics RPC
        public override Task<LyricsOvh_Reply> GetLyricWithDatabaseHelp(LyricsOvh_Request request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
