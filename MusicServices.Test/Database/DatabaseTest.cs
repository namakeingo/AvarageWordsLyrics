using Microsoft.VisualStudio.TestTools.UnitTesting;

using MusicServices.Database;

namespace MusicServices.Test.Database
{
    [TestClass]
    public class DatabaseTest
    {
        /// <summary>
        /// Test to get the lyric of a song from the local database
        /// This TestMethod testes the fuctionality to the entire partial LocalStoreDatabase.Lyric
        /// Get/Insert/Delete are codependent so they cannot be tested separately
        /// </summary>
        [TestMethod]
        public void Database_Lyric()
        {
            LocalStoreDatabase database = new LocalStoreDatabase();

            //Lyrics test objects to insert
            string identifier = "Database_Lyric";
            LyricsOvh.LyricsOvh_Request test_request = new LyricsOvh.LyricsOvh_Request
            {
                ArtistName = identifier,
                SongTitle = identifier
            };

            LyricsOvh.LyricsOvh_Reply test_response = new LyricsOvh.LyricsOvh_Reply()
            {
                LyricText = @"Part to remove\r\nDatabase_Lyric unit test lyric words"
            };

            //Clean lyric parameter and words count for assert
            string lyricClean = @"Database_Lyric unit test lyric words";
            int wordsCount = 5;

            //Store actual size of the Database lyric table before inserting
            int sizeBeforeInsert = database.myStoresLyricsSize();

            //Insert lyric using test data
            StoredLyric? inserted = database.InsertLyric(test_request, ref test_response);
            Assert.IsTrue(inserted != null);

            //Store new size of the Database lyric table after 1 row was inserting
            int sizeAfterInsert = database.myStoresLyricsSize();
            //Verify we have an additional row
            Assert.IsTrue(sizeAfterInsert == (sizeBeforeInsert + 1));

            //Get inserted lyric
            inserted = null;
            inserted = database.GetLyric(test_request);
            //Verify it matches test objects
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.ArtistName == test_request.ArtistName);
            Assert.IsTrue(inserted.SongTitle == test_request.SongTitle);
            Assert.IsTrue(inserted.LyricText == lyricClean);
            Assert.IsTrue(inserted.LyricWordsCount == wordsCount);

            //Discard Database and reload from file
            database.Dispose();
            database = new LocalStoreDatabase();

            //Verify database size is complete
            Assert.IsTrue(database.myStoresLyricsSize() == sizeAfterInsert);

            //Get inserted lyric after the Database was reloaded to verify that data was stored and retrived correctly
            inserted = null;
            inserted = database.GetLyric(test_request);
            Assert.IsTrue(inserted != null);
            Assert.IsTrue(inserted.ArtistName == test_request.ArtistName);
            Assert.IsTrue(inserted.SongTitle == test_request.SongTitle);
            Assert.IsTrue(inserted.LyricText == lyricClean);
            Assert.IsTrue(inserted.LyricWordsCount == wordsCount);

            //Remove test lyric
            database.DeleteLyric(test_request);

            //Try to get deleted row
            inserted = null;
            inserted = database.GetLyric(test_request);
            //Veryfy row was deleted succesfully
            Assert.IsTrue(inserted == null);
            Assert.IsTrue(database.myStoresLyricsSize() == sizeBeforeInsert);
        }
    }
}
