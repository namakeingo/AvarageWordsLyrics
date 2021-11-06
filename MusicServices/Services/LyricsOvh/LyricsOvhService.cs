using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Core;

using LyricsOvh;
using MusicServices.Database;
using MusicServices.Services.Shared;
using MusicServices.DataContracts.LyricsOvh;

namespace MusicServices.Services.LyricsOvh
{
    public class LyricsOvhService : LyricsOvhProto.LyricsOvhProtoBase
    {
        private const string API_BASE_URL = "https://api.lyrics.ovh/v1/{0}/{1}";

        private static HttpClient httpClient;
        public static LocalStoreDatabase Database; 
        public LyricsOvhService()
        {
            httpClient = new HttpClient();
            Database = new LocalStoreDatabase();
        }

#nullable enable

        /// <summary>
        /// Server Side handler of the GetLyrics RPC
        /// Retrieve the lyric of the song using https://api.lyrics.ovh/v1/artist/title
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<LyricsOvh_Reply> GetLyric(LyricsOvh_Request request, ServerCallContext context)
        {
            //Format get request url
            string requestUrl = string.Format(API_BASE_URL, request.ArtistName, request.SongTitle);

            LyricsOvh_Type rawResponse = REST.Get<LyricsOvh_Type>(httpClient, requestUrl).Result;

            LyricsOvh_Reply reply = new LyricsOvh_Reply();

            Database.RawLyricToCleanReply(rawLyric: rawResponse.lyrics, ref reply);

            return Task.FromResult(reply);
        }

        /// <summary>
        /// Server Side handler of the GetLyricWithDatabaseHelp RPC
        /// Retrieve the lyric of the song using the database.
        /// If not found in database it is searched using https://api.lyrics.ovh/v1/artist/title
        /// Response is added to the database for the next identical request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<LyricsOvh_Reply> GetLyricWithDatabaseHelp(LyricsOvh_Request request, ServerCallContext context)
        {
            LyricsOvh_Reply reply = new LyricsOvh_Reply();

            StoredLyric? storedLyric = Database.GetLyric(request);
            if(storedLyric == null)
            {
                //Format get request url
                string requestUrl = string.Format(API_BASE_URL, request.ArtistName, request.SongTitle);

                LyricsOvh_Type rawResponse = REST.Get<LyricsOvh_Type>(httpClient, requestUrl).Result;
                reply.LyricText = rawResponse.lyrics;

                storedLyric = Database.InsertLyric(request, ref reply);
            }
            else {
                reply.LyricText = storedLyric.LyricText;
                reply.LyricWordsCount = storedLyric.LyricWordsCount;
            }

            return Task.FromResult(reply);
        }
    }
}
