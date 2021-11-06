using NUnit.Framework;
using System.Linq;

using MusicServices.Test.TestingHelpers;
using MusicServices.Services.MusicBrainz;

namespace MusicServices.Test.Services
{
    public class MusicBrainzServiceTest
    {
        public static MusicBrainzService service = new MusicBrainzService();
        /// <summary>
        /// Test to serch artists with a simple artist name
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtist()
        {
            MusicBrainz.MusicBrainz_SearchArtist_Request request
                = new MusicBrainz.MusicBrainz_SearchArtist_Request { ArtistName = "eminem" };

            MusicBrainz.MusicBrainz_SearchArtist_Reply response
                = service.SearchArtist(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Artists.Any(x => x.ArtistID == "b95ce3ff-3d05-4e87-9e01-c97b66af13d4"));
        }

        /// <summary>
        /// Test to serch artists with a artist name that has spaces
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtist_WithSpace()
        {
            MusicBrainz.MusicBrainz_SearchArtist_Request request
                = new MusicBrainz.MusicBrainz_SearchArtist_Request { ArtistName = "twenty one pilots" };

            MusicBrainz.MusicBrainz_SearchArtist_Reply response
                = service.SearchArtist(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Artists.Any(x => x.ArtistID == "a6c6897a-7415-4f8d-b5a5-3a5e05f3be67"));
        }

        /// <summary>
        /// Test to serch artists with a artist name that has characters that might need escaping
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtist_WithEscapeChars()
        {
            MusicBrainz.MusicBrainz_SearchArtist_Request request
                = new MusicBrainz.MusicBrainz_SearchArtist_Request { ArtistName = "SelloRekT / LA Dreams" };

            MusicBrainz.MusicBrainz_SearchArtist_Reply response
                = service.SearchArtist(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Artists.Any(x => x.ArtistID == "a7bdd428-cd21-4e86-b775-ea54a9629527"));
        }

        /// <summary>
        /// Test prevalidation for SearchArtist with less then 3 characters in ArtistName
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtist_Min3CharArtist()
        {
            MusicBrainz.MusicBrainz_SearchArtist_Request request
                = new MusicBrainz.MusicBrainz_SearchArtist_Request { ArtistName = "12" };

            MusicBrainz.MusicBrainz_SearchArtist_Reply response
                = service.SearchArtist(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.HasError);
            Assert.IsTrue(response.ErrorMessage == "ArtistName should be 3 characters long or more");
        }

        /// <summary>
        /// Test search of songs by artistID
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtistSongs()
        {
            MusicBrainz.MusicBrainz_SearchArtistSongs_Request request
                = new MusicBrainz.MusicBrainz_SearchArtistSongs_Request
                {
                    ArtistID = "a6c6897a-7415-4f8d-b5a5-3a5e05f3be67",
                    ArtistName = "twenty one pilots"
                };

            MusicBrainz.MusicBrainz_SearchArtistSongs_Reply response
                = service.SearchArtistSongs(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Songs.Any(x => x.SongTitle == "The Pantaloon"));
            Assert.IsTrue(response.Songs.Any(x => x.SongTitle == "Stressed Out"));
            Assert.IsTrue(response.Songs.Any(x => x.SongTitle == "Message Man"));
        }

        /// <summary>
        /// Test prevalidation for SearchArtistSongs with less then 3 characters in ArtistName
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtistSongs_Min3CharArtist()
        {
            MusicBrainz.MusicBrainz_SearchArtistSongs_Request request
                = new MusicBrainz.MusicBrainz_SearchArtistSongs_Request
                {
                    ArtistID = "not-really-an-id-d-b5a5-3a5e05f3be67",
                    ArtistName = "12"
                };

            MusicBrainz.MusicBrainz_SearchArtistSongs_Reply response
                = service.SearchArtistSongs(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.HasError);
            Assert.IsTrue(response.ErrorMessage == "ArtistName should be 3 characters long or more");
        }

        /// <summary>
        /// Test prevalidation for SearchArtistSongs ArtistID not of 36 characters
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtistSongs_36CharArtistID()
        {
            MusicBrainz.MusicBrainz_SearchArtistSongs_Request request
                = new MusicBrainz.MusicBrainz_SearchArtistSongs_Request
                {
                    ArtistID = "b95ce3ff-3d05-wrong-length",
                    ArtistName = "eminem"
                };

            MusicBrainz.MusicBrainz_SearchArtistSongs_Reply response
                = service.SearchArtistSongs(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.HasError);
            Assert.IsTrue(response.ErrorMessage == "ArtistID should always be exactly 36 characters");
        }
    }
}
