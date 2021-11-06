using NUnit.Framework;
using System.Linq;

using MusicServices.Test.TestingHelpers;
using MusicServices.Services.MusicBrainz;

namespace MusicServices.Test.Services
{
    public class MusicBrainzServiceTest
    {
        /// <summary>
        /// Test to serch artists with a simple artist name
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtist()
        {
            MusicBrainzService service = new MusicBrainzService();
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
            MusicBrainzService service = new MusicBrainzService();
            MusicBrainz.MusicBrainz_SearchArtist_Request request
                = new MusicBrainz.MusicBrainz_SearchArtist_Request { ArtistName = "twenty one pilots" };

            MusicBrainz.MusicBrainz_SearchArtist_Reply response
                = service.SearchArtist(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Artists.Any(x => x.ArtistID == "a6c6897a-7415-4f8d-b5a5-3a5e05f3be67"));
        }

        /// <summary>
        /// Test to serch artists with a artist name that has characters that need escaping
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtist_WithEscapeChars()
        {
            MusicBrainzService service = new MusicBrainzService();
            MusicBrainz.MusicBrainz_SearchArtist_Request request
                = new MusicBrainz.MusicBrainz_SearchArtist_Request { ArtistName = "JAY-Z" };

            MusicBrainz.MusicBrainz_SearchArtist_Reply response
                = service.SearchArtist(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Artists.Any(x => x.ArtistID == "f82bcf78-5b69-4622-a5ef-73800768d9ac"));
        }

        /// <summary>
        /// Test search of songs by artistID
        /// </summary>
        [Test]
        public void MusicBrainzTest_SearchArtistSongs()
        {
            MusicBrainzService service = new MusicBrainzService();
            MusicBrainz.MusicBrainz_SearchArtistSongs_Request request
                = new MusicBrainz.MusicBrainz_SearchArtistSongs_Request { ArtistID = "a6c6897a-7415-4f8d-b5a5-3a5e05f3be67", ArtistName = "twenty one pilots" };

            MusicBrainz.MusicBrainz_SearchArtistSongs_Reply response
                = service.SearchArtistSongs(request, gRPC.CreateTestContext()).Result;

            Assert.IsTrue(response.Songs.Any(x => x.SongTitle == "The Pantaloon"));
            Assert.IsTrue(response.Songs.Any(x => x.SongTitle == "Stressed Out"));
            Assert.IsTrue(response.Songs.Any(x => x.SongTitle == "Message Man"));
        }
    }
}
