using System;
using System.Collections.Generic;
using System.Linq;

using LyricsOvh;

namespace MusicServices.Database
{
	public partial class LocalStoreDatabase
	{
		/// <summary>
		/// Get the Count property form myStoredLyrics
		/// </summary>
		/// <returns>myStoredLyrics.Count()</returns>
		public int myStoresLyricsSize()
        {
			return myStoredLyrics.Count();
        }

		/// <summary>
		/// Get a StoredLyric from myStoredLyrics that matches the request
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Matching StoredLyric or null</returns>
		public StoredLyric? GetLyric(LyricsOvh_Request request)
		{
			return myStoredLyrics.FindOne(x => x.ArtistName == request.ArtistName && x.SongTitle == request.SongTitle);
		}

		/// <summary>
		/// Insert request respons
		/// It will also remove useless parts form the LyricsOvh_Reply
		/// and it calculate the LyricWordsCount
		/// </summary>
		/// <param name="request"></param>
		/// <param name="lyric"></param>
		/// <returns>Matching StoredLyric or null</returns>
		public StoredLyric? InsertLyric(LyricsOvh_Request request, ref LyricsOvh_Reply lyric)
		{
			//get rid of useless parts form the LyricsOvh_Reply
			lyric.LyricText = lyric.LyricText.Split("\r\n", 2, StringSplitOptions.RemoveEmptyEntries)[1];

			//Separate and count words
			string[] separators = { "\n", " ", ",", "'" };
			List<string> words = lyric.LyricText.Split(separators, int.MaxValue, System.StringSplitOptions.RemoveEmptyEntries).ToList();
			lyric.LyricWordsCount = words.Count;

			myStoredLyrics.Insert(new StoredLyric()
			{
				ArtistName = request.ArtistName,
				SongTitle = request.SongTitle,
				LyricText = lyric.LyricText,
				LyricWordsCount = lyric.LyricWordsCount
			});

			return GetLyric(request);
		}

		public void DeleteLyric(LyricsOvh_Request request)
        {
			myStoredLyrics.DeleteMany(x => x.ArtistName == request.ArtistName && x.SongTitle == request.SongTitle);
        }
	}
}
