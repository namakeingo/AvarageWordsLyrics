using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

using MusicBrainz;
using MusicServices.DataContracts.MusicBrainz;
using MusicServices.Services.Shared;

namespace MusicServices.Services.MusicBrainz
{
    public class MusicBrainzService : MusicBrainzProto.MusicBrainzProtoBase
    {
        const string API_BASE_URL = "http://musicbrainz.org/ws/2/";
        const string ARTIST_URL = API_BASE_URL + "artist/?query=artist:\"{0}\"&offset={1}";
        const string SONGS_URL = API_BASE_URL + "recording/?query=arid:\"{0}\" AND type:\"Album\" NOT recording:\"\\(Instrumental\\)\" NOT recording:\"Remix\"&offset={1}";

        private static HttpClient httpClient;
        public MusicBrainzService()
        {
            httpClient = new HttpClient();
        }
        // Server Side handler of the GetLyrics RPC
        public override Task<MusicBrainz_SearchArtist_Reply> SearchArtist(MusicBrainz_SearchArtist_Request request, ServerCallContext context)
        {
            MusicBrainz_SearchArtist_Reply reply = new MusicBrainz_SearchArtist_Reply();

            reply = MusicBrainz_Prevalidate(request, reply);

            int count = 1;
            //Use offset and count of the api to populate the response with all the rows
            while (count > reply.Artists.Count 
                && (!request.Limit.HasValue || request.Limit > reply.Artists.Count) 
                && !reply.HasError)
            {
                //Format get request url
                string requestUrl = string.Format(ARTIST_URL, request.ArtistName
                    , reply.Artists.Count + (request.Offset.HasValue ? request.Offset.Value : 0));

                try
                {
                    MusicBrainz_Type rawResponse = REST.Get<MusicBrainz_Type>(httpClient, requestUrl).Result;
                    count = rawResponse.count;
                    reply.ResultsCount = count;

                    foreach (MusicBrainz_Artist_Type artist in rawResponse.artists)
                    {
                        string contry = null;
                        //Get full contry name from 2 letter ISO
                        if (!string.IsNullOrWhiteSpace(artist.country))
                        {
                            var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(x => new RegionInfo(x.LCID));
                            var region = regions.FirstOrDefault(r => r.TwoLetterISORegionName.Contains(artist.country));
                            contry = region.EnglishName;
                        }

                        reply.Artists.Add(new MusicBrainz_Artist()
                        {
                            ID = artist.id,
                            Name = artist.name,
                            CountryTwoLetterISO = artist.country,
                            Country = contry,
                            Disambiguation = artist.disambiguation,
                            Gender = artist.gender,
                            Score = artist.score,
                            Type = (MusicBrainz_ArtistTypeEnum)artist.type
                        }); ;
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("503 (Service Temporarily Unavailable)"))
                    {
                        //Wait for 500ms before twying to get next page again
                        Thread.Sleep(500);
                    }
                    else
                    {
                        reply.HasError = true;
                        reply.ErrorMessage = e.Message;
                    }
                }
            }

            return Task.FromResult(reply);
        }
        // Server Side handler of the GetLyrics RPC
        public override Task<MusicBrainz_SearchArtistSongs_Reply> SearchArtistSongs(MusicBrainz_SearchArtistSongs_Request request, ServerCallContext context)
        {
            MusicBrainz_SearchArtistSongs_Reply reply = new MusicBrainz_SearchArtistSongs_Reply();

            reply = MusicBrainz_Prevalidate(request, reply);

            //Declare variables to calculate Stats
            int totalLength = 0;
            int minLength = int.MaxValue;
            int maxLength = 0;
            reply.SongsStats = new MusicBrainz_SongsStats();

            int count = 1;
            int offset = 0;
            //Use offset and count of the api to populate the response with all the rows
            while (count > offset && !reply.HasError)
            {
                //Format get request url
                string requestUrl = string.Format(SONGS_URL, request.ArtistID, offset);

                try
                {
                    MusicBrainz_Type rawResponse = REST.Get<MusicBrainz_Type>(httpClient, requestUrl).Result;
                    count = rawResponse.count;

                    foreach (MusicBrainz_Recording_Type recording in rawResponse.recordings)
                    {
                        offset++;

                        //Only insert if we don't have that song title already.
                        //The same song can have multiple recordings
                        if (!reply.Songs.Any(x => x.Title == recording.title))
                        {
                            MusicBrainz_Song song = new MusicBrainz_Song()
                            {
                                ArtistName = request.ArtistName,
                                ID = recording.id,
                                Title = recording.title,
                                Score = recording.score,
                                Length = recording.length
                            };
                            reply.Songs.Add(song);

                            totalLength = totalLength + song.Length;
                            if (song.Length > maxLength) {
                                maxLength = song.Length;
                                reply.SongsStats.MaxLengthSong = song;
                            }
                            if (song.Length < minLength) {
                                minLength = song.Length;
                                reply.SongsStats.MinLengthSong = song;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("503 (Service Temporarily Unavailable)"))
                    {
                        //Wait for 500 milliseconds before twying to get next page again
                        Thread.Sleep(500);
                    }
                    else
                    {
                        reply.HasError = true;
                        reply.ErrorMessage = e.Message;
                    }
                }
            }

            //Calculate and store avarage length
            if (reply.Songs.Count != 0)
            {
                reply.SongsStats.AvarageLength = totalLength / reply.Songs.Count;
            }

            return Task.FromResult(reply);
        }

        public dynamic MusicBrainz_Prevalidate(dynamic request, dynamic reply)
        {
            if (request is MusicBrainz_SearchArtistSongs_Request && request.ArtistID.Length != 36)
            {
                reply.HasError = true;
                reply.ErrorMessage = "ArtistID should always be exactly 36 characters";
            }
            else if (request.ArtistName.Length < 3)
            {
                reply.HasError = true;
                reply.ErrorMessage = "ArtistName should be 3 characters long or more";
            }
            return reply;
        }
    }
}
