using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

using MusicServices.Test.TestingHelpers;
using MusicServices.Services.LyricsOvh;

namespace MusicServices.Test.Services
{
    public class LyricsOvhServiceTest
    {
        /// <summary>
        /// Test to get the lyric of a song
        /// </summary>
        [Test]
        public void LyricsOvhTest_GetLyric()
        {
            LyricsOvhService service = new LyricsOvhService();
            LyricsOvh.LyricsOvh_Request request
                = new LyricsOvh.LyricsOvh_Request { ArtistName = "twenty one pilots", SongTitle = "Car Radio" };

            LyricsOvh.LyricsOvh_Reply response
                = service.GetLyric(request, gRPC.CreateTestContext()).Result;

            string lyricDirty = "Paroles de la chanson Car Radio par Twenty One Pilots\r\nI ponder of something great\nMy lungs will fill and then deflate\nThey fill with fire\nExhale desire\nI know it's dire\nMy time today\n\nI have these thoughts\nSo often I ought\nTo replace that slot\nWith what I once bought\n'Cause somebody stole\nMy car radio\nAnd now I just sit in silence\n\nSometimes quiet is violent\n\nI find it hard to hide it\nMy pride is no longer inside\nIt's on my sleeve\nMy skin will scream\nReminding me of\nWho I killed inside my dream\nI hate this car that I'm driving\nThere's no hiding for me\nI'm forced to deal with what I feel\nThere is no distraction to mask what is real\nI could pull the steering wheel\n\nI have these thoughts\nSo often I ought\nTo replace that slot\nWith what I once bought\n'Cause somebody stole\nMy car radio\n\nAnd now I just sit in silence\n\nI ponder of something terrifying\n'Cause this time there's no sound to hide behind\nI find over the course of our human existence\nOne thing consists of consistence\nAnd it's that we're all battling fear\nOh dear, I don't know if we know why we're here\nOh my,\nToo deep\nPlease stop thinking\nI liked it better when my car had sound\n\nThere are things we can do\nBut from the things that work there are only two\nAnd from the two that we choose to do\nPeace will win\nAnd fear will lose\n\nThere's faith and there's sleep\nWe need to pick one please because\nFaith is to be awake\nAnd to be awake is for us to think\nAnd for us to think is to be alive\nAnd I will try with every rhyme\nTo come across like I am dying\nTo let you know you need to try to think\n\nI have these thoughts\nSo often I ought\nTo replace that slot\nWith what I once bought\n'Cause somebody stole\nMy car radio\nAnd now I just sit in silence\n\nI ponder of something great\n\nMy lungs will fill and then deflate\nThey fill with fire\nExhale desire\nI know it's dire\nMy time today\n\nI have these thoughts\nSo often I ought\nTo replace that slot\nWith what I once bought\n'Cause somebody stole\nMy car radio\nAnd now I just sit in silence";

            string lyricClean = lyricDirty.Replace("Paroles de la chanson Car Radio par Twenty One Pilots\r\n", "");

            string[] separators = { "\n", " ", ",", "'" };
            List<string> words = lyricClean.Split(separators, int.MaxValue, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            int wordCount = words.Count;

            Assert.IsTrue(response.LyricText == lyricClean);
            Assert.IsTrue(response.LyricWordsCount == wordCount);

            LyricsOvhService.Database.Dispose();
        }

        /// <summary>
        /// Test to get the lyric of a song that we already have in the database
        /// </summary>
        [Test]
        public void LyricsOvhTest_GetLyric_FromDatabase()
        {
            string identifier = "LyricsOvhTest_GetLyric_FromDatabase";

            LyricsOvh.LyricsOvh_Request request = new LyricsOvh.LyricsOvh_Request
            {
                ArtistName = identifier,
                SongTitle = identifier
            };

            LyricsOvh.LyricsOvh_Reply test_response = new LyricsOvh.LyricsOvh_Reply()
            {
                LyricText = "Part to remove\r\nLyricsOvhTest_GetLyric_FromDatabase unit test lyric words"
            };

            string lyricClean = "LyricsOvhTest_GetLyric_FromDatabase unit test lyric words";
            int wordCount = 5;

            LyricsOvhService service = new LyricsOvhService();

            //Add test data to database
            LyricsOvhService.Database.InsertLyric(request, ref test_response);

            //Should retrieve using database so the response should match our fake response we inserted
            LyricsOvh.LyricsOvh_Reply response
                = service.GetLyricWithDatabaseHelp(request, gRPC.CreateTestContext()).Result;

            //Remove test data from database
            LyricsOvhService.Database.DeleteLyric(request);

            Assert.IsTrue(response.LyricText == lyricClean);
            Assert.IsTrue(response.LyricWordsCount == wordCount);

            LyricsOvhService.Database.Dispose();
        }
    }
}
