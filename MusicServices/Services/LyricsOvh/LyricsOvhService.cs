using System;
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
            LyricsOvh_Reply reply = new LyricsOvh_Reply();

            LyricsOvh_Prevalidate(request, ref reply);
            if (!reply.HasError)
            {
                try
                {
                    //Format get request url
                    string requestUrl = string.Format(API_BASE_URL, request.ArtistName, request.SongTitle);

                    LyricsOvh_Type rawResponse = REST.Get<LyricsOvh_Type>(httpClient, requestUrl).Result;

                    Database.RawLyricToCleanReply(rawLyric: rawResponse.lyrics, ref reply);
                }
                catch (Exception e)
                {
                    reply.HasError = true;
                    reply.ErrorMessage = e.Message;
                }
            }

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

            LyricsOvh_Prevalidate(request, ref reply);
            if (!reply.HasError)
            {
                try
                {
                    StoredLyric? storedLyric = Database.GetLyric(request);
                    if (storedLyric == null)
                    {
                        //Format get request url
                        string requestUrl = string.Format(API_BASE_URL, request.ArtistName, request.SongTitle);

                        LyricsOvh_Type rawResponse = REST.Get<LyricsOvh_Type>(httpClient, requestUrl).Result;
                        reply.LyricText = rawResponse.lyrics;

                        storedLyric = Database.InsertLyric(request, ref reply);
                    }
                    else
                    {
                        reply.LyricText = storedLyric.LyricText;
                        reply.LyricWordsCount = storedLyric.LyricWordsCount;
                    }
                }
                catch (Exception e)
                {
                    reply.HasError = true;
                    reply.ErrorMessage = e.Message;
                }
            }

            return Task.FromResult(reply);
        }

        public void LyricsOvh_Prevalidate(LyricsOvh_Request request, ref LyricsOvh_Reply reply)
        {
            if (request.ArtistName.Contains("/") || request.ArtistName.Contains("\\")
                || request.SongTitle.Contains("/") || request.SongTitle.Contains("\\"))
            {
                reply.HasError = true;
                reply.ErrorMessage = "Unfortunately api.lyrics.ovh does not supports artist/songs with '\\' or '/' in their name/title";
            }
            else if (string.IsNullOrWhiteSpace(request.ArtistName) || string.IsNullOrWhiteSpace(request.SongTitle)){
                reply.HasError = true;
                reply.ErrorMessage = "ArtistName and/or SongTitle should not be empty";
            }
        }
    }
}
