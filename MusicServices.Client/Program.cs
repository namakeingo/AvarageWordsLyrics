using System;
using Grpc.Core;

using MusicBrainz;
using LyricsOvh;
using System.Threading.Tasks;

namespace MusicServices.Cliant
{
    class Program
    {
        private static Channel channel = new Channel("127.0.0.1:30052", ChannelCredentials.Insecure);

        /// <summary>
        /// Start of Console App
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:30052", ChannelCredentials.Insecure);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Welcome");
            Console.WriteLine("\nThis Console Application will let you retrieve statistical information about a music artist of your choice.");
            Console.WriteLine("Please be aware that due to the fact that we rely to a slow API the response can be really slow");

            /// Keep process open
            while (true)
            {
                ArtistInput();
            }
        }

        /// <summary>
        /// Section of the console app allowing for input of Artist Name
        /// </summary>
        private static void ArtistInput()
        {
            Console.WriteLine("\nPlease type the name of the artist, then press enter to proceed...");
            string artistName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(artistName))
            {
                Console.WriteLine("The artist name should not be Empty or only white spaces");
                //App will go back to start of outer while loop in Main()
            }
            else if (artistName.Length < 3)
            {
                Console.WriteLine("The artist name should be at least 3 characters long");
                //App will go back to start of outer while loop in Main()
            }
            else
            {
                MusicBrainz_SearchArtist_Request saRequst = new MusicBrainz_SearchArtist_Request()
                {
                    ArtistName = artistName,
                    Limit = 50,
                    Offset = 0
                };

                //search artist using MusicBrainz service
                MusicBrainzProto.MusicBrainzProtoClient client_MusicBrainz = new MusicBrainzProto.MusicBrainzProtoClient(channel);
                MusicBrainz_SearchArtist_Reply saReply = client_MusicBrainz.SearchArtist(saRequst);

                if (saReply.HasError)
                {
                    Console.WriteLine("Sorry there was a problem comunicating with MusicBrainz");
                    Console.WriteLine(saReply.ErrorMessage);
                    //App will go back to start of outer while loop in Main()
                }
                else
                {
                    switch (saReply.ResultsCount)
                    {
                        case 0:
                            Console.WriteLine(
                                String.Format("Sorry it looks like there is no artist in MusicBrainz for: \"{0}\"", artistName));
                            //App will go back to start of outer while loop in Main()
                            break;
                        case 1:
                            //Proceeds to next section
                            OneArtistSelected(saReply.Artists[0]);
                            break;
                        case <= 50:
                            for (int i = 0; i < saReply.Artists.Count; i++)
                            {
                                Console.WriteLine(
                                String.Format("{0} - artist:\"{1}\" - https://musicbrainz.org/artist/{2}", i, saReply.Artists[i].Name, saReply.Artists[i].ID));
                            }
                            //Proceeds to next section to let the user select form multiple artists
                            SelectFromMultipleArtists(saReply);
                            break;
                        case > 50:
                            Console.WriteLine(
                                String.Format("There are {0} results in MusicBrainz for: \"{1}\"", saReply.ResultsCount, artistName));
                            Console.WriteLine("Please try to be more specific");
                            //App will go back to start of outer while loop in Main()
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Section of the console app allowing for selecting betwen multiple artists found from the search
        /// </summary>
        /// <param name="reply"></param>
        private static void SelectFromMultipleArtists(MusicBrainz_SearchArtist_Reply reply)
        {
            Console.WriteLine("\nType the number corresponding to the desired artist");
            Console.WriteLine("or enter \"start\" to go back...");

            string line;

            line = Console.ReadLine();
            if (line != null && line != "")
            {
                if (line.ToLower() == "start")
                {
                    //App will go back to start of outer while loop in Main()
                }
                else if (Int32.TryParse(line.Trim(), out int selectedIndex) && selectedIndex < reply.Artists.Count)
                {
                    //Go to nect section so that the artist info can be outputted
                    OneArtistSelected(reply.Artists[selectedIndex], selectedIndex);
                }
                else
                {
                    Console.WriteLine(String.Format("\"{0}\" is not a valid input", line));
                    //Go back to start of current section
                    SelectFromMultipleArtists(reply);
                }
            }
            else
            {
                Console.WriteLine("Input cannot be empty");
                //Go back to start of current section
                SelectFromMultipleArtists(reply);
            }
        }

        /// <summary>
        /// Section of the console app dedicated to outputting the results for a specific artist and retrieve the songs
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="index"></param>
        private static void OneArtistSelected(MusicBrainz_Artist artist, int? index = null)
        {
            //Output artist informations
            Console.WriteLine(
                        String.Format("Selected: {0}artist:\"{1}\" - https://musicbrainz.org/artist/{2}",
                        index.HasValue ? string.Format("{0} - ", index.Value) : "",
                        artist.Name,
                        artist.ID));
            Console.WriteLine(String.Format("This artist is a {0}", artist.Type.ToString()));
            Console.WriteLine(String.Format("They are scored {0} on MusicBrainz", artist.Score));
            if (!string.IsNullOrWhiteSpace(artist.Country))
            {
                Console.WriteLine(String.Format("The country of origin is {0}", artist.Country));
            }
            if (!string.IsNullOrWhiteSpace(artist.Gender))
            {
                Console.WriteLine(String.Format("The artist gender is {0}", artist.Gender));
            }
            if (!string.IsNullOrWhiteSpace(artist.Disambiguation))
            {
                Console.WriteLine(String.Format("They can be described as {0}", artist.Disambiguation));
            }

            //Now start songs retrival
            Console.WriteLine("\nLooking for songs. This can take a while. Please wait...");

            MusicBrainz_SearchArtistSongs_Request sasRequst = new MusicBrainz_SearchArtistSongs_Request()
            {
                ArtistID = artist.ID,
                ArtistName = artist.Name
            };

            //search songs using MusicBrainz service
            MusicBrainzProto.MusicBrainzProtoClient client_MusicBrainz = new MusicBrainzProto.MusicBrainzProtoClient(channel);
            MusicBrainz_SearchArtistSongs_Reply sasReply = client_MusicBrainz.SearchArtistSongs(sasRequst);

            if (sasReply.HasError)
            {
                Console.WriteLine("Sorry there was a problem comunicating with MusicBrainz");
                Console.WriteLine(sasReply.ErrorMessage);
                //App will go back to start of outer while loop in Main()
            }
            else
            {
                switch (sasReply.Songs.Count)
                {
                    case 0:
                        Console.WriteLine(
                            String.Format("\nSorry it looks like MusicBrainz has no songs for: \"{0}\"", artist.Name));
                        //App will go back to start of outer while loop in Main() in Main()
                        break;
                    case 1:
                        //Output messages for one artist with only one song
                        Console.WriteLine(
                            String.Format("\n\"{0}\" has only one song.", artist.Name));

                        //Using TimeSpan to get string format of time
                        TimeSpan ts = TimeSpan.FromMilliseconds(sasReply.Songs[0].Length);

                        Console.WriteLine(
                             String.Format("The song is titled \"{0}\" and it is {1} minutes long.",
                             sasReply.Songs[0].Title,
                             ts.ToString(@"mm\:ss")));
                        Console.WriteLine(
                             String.Format("The song can be found at https://musicbrainz.org/recording/{0}", sasReply.Songs[0].ID));

                        //Get lyric for lirics.ovh
                        LyricsOvh_Reply lReply = GetLyrics(sasReply.Songs[0]).Result;

                        if (lReply.HasError)
                        {
                            Console.WriteLine("No lyrics found");
                            //App will go back to start of outer while loop in Main()
                        }
                        else
                        {
                            //Output info for 1 lyric
                            Console.WriteLine(
                             String.Format("The song lyrics words count is: {0}", lReply.LyricWordsCount));
                            Console.WriteLine(
                             String.Format("The lyrics full text is: \n{0}", lReply.LyricText));
                        }
                        break;
                    case > 1:
                        Console.WriteLine(
                            String.Format("\n\"{0}\" has a total of {1} songs.", artist.Name, sasReply.Songs.Count));
                        //proceeds to next section
                        ConfirmBeforeLyricsSearch(sasReply);
                        break;
                }
            }
        }

        /// <summary>
        /// Section of the console app that handle the user confirmation before proceeding with lyric.ovh comunications
        /// </summary>
        /// <param name="sasReply"></param>
        private static void ConfirmBeforeLyricsSearch(MusicBrainz_SearchArtistSongs_Reply sasReply)
        {
            //Calculate approx. time the lyric api could take
            //Using TimeSpan to get string format of time
            TimeSpan ts = TimeSpan.FromMilliseconds(sasReply.Songs.Count * 300);
            Console.WriteLine(String.Format("\nThe lyrics search could take up to {0} minutes. Are you sure you want to proceed? (y/n)",
                                    ts.ToString(@"mm\:ss")));

            string line = Console.ReadLine();
            if (line != null && line != "")
            {
                if (line.ToLower() == "n")
                {
                    //App will go back to start of outer while loop in Main()
                }
                else if (line.ToLower() == "y")
                {
                    //Proceeds to next section to handle songs informations output
                    SongsFound(sasReply);
                    //App will go back to start of outer while loop in Main()
                }
                else
                {
                    Console.WriteLine(String.Format("\"{0}\" is not a valid input", line));
                    //Go back to start of current section
                    ConfirmBeforeLyricsSearch(sasReply);
                }
            }
            else
            {
                Console.WriteLine("Input cannot be empty");
                //Go back to start of current section
                ConfirmBeforeLyricsSearch(sasReply);
            }
        }

        /// <summary>
        /// Section of the console app that outputs informations regarding songs from a list of them
        /// </summary>
        /// <param name="mReply"></param>
        private static void SongsFound(MusicBrainz_SearchArtistSongs_Reply sasReply)
        {
            int avarage;
            decimal avarageWPM;
            int totalWords = 0;
            int totalLength = 0;
            int maxWords = 0;
            int minWords = int.MaxValue;
            decimal maxWPM = 0;
            decimal minWPM = int.MaxValue;
            bool hasNoSuccesResponse = true;

            Console.WriteLine(String.Format("\nSearching Lyrics... 0 / {0}\n\n\n\n\n", sasReply.Songs.Count));
            for (int i = 0; i < sasReply.Songs.Count; i++)
            {
                MusicBrainz_Song song = sasReply.Songs[i];
                LyricsOvh_Reply lReply = GetLyrics(song).Result;

                //Move cursors up by 6 lines so that we write on top of the previous lines
                Console.SetCursorPosition(0, Console.CursorTop - 6);

                if (lReply.HasError && hasNoSuccesResponse)
                {
                    Console.WriteLine(String.Format("Searching Lyrics... {0} / {1}\n\nNo lyrics found\n\n\n",
                        1+i,
                        sasReply.Songs.Count));
                }
                else
                {
                    hasNoSuccesResponse = false;

                    //Calculate lyrics stats
                    totalWords = totalWords + lReply.LyricWordsCount;
                    totalLength = totalLength + song.Length;
                    avarage = totalWords / (1 + i);
                    avarageWPM = (decimal)totalWords / ((decimal)totalLength / 1000 / 60);
                    if (lReply.LyricWordsCount > maxWords)
                    {
                        maxWords = lReply.LyricWordsCount;
                        //Calculate max words per minute of this song
                        decimal wpm = (decimal)lReply.LyricWordsCount / ((decimal)song.Length / 1000 / 60);
                        maxWPM = wpm > maxWPM ? wpm : maxWPM;
                    }
                    if (lReply.LyricWordsCount != 0 && lReply.LyricWordsCount < minWords)
                    {
                        //Ignore 0 for minimum as it is not interesting for this stat
                        minWords = lReply.LyricWordsCount;
                        //Calculate max words per minute of this song
                        decimal wpm = (decimal)lReply.LyricWordsCount / ((decimal)song.Length / 1000 / 60);
                        minWPM = wpm < minWPM ? wpm : minWPM;
                    }

                    //Write line on to of previus lines. Spaces are necessary to overwrite all that was written on that line
                    Console.WriteLine(
                        String.Format("Searching Lyrics... {0} / {1} ...                              " +
                        "\n                                                                           " +
                        "\nLyrics stats:                                                              " +
                        "\nWords avarage is {2} with an avarege words per minute of {3}               " +
                        "\nMin words count is {4} with a minimum words per minute of {5}              " +
                        "\nMax words count is {6} with a maximum words per minute of {7}              ",
                            1 + i,
                            sasReply.Songs.Count,
                            avarage.ToString("0.00"),
                            avarageWPM,
                            minWords,
                            minWPM.ToString("0.00"),
                            maxWords,
                            maxWPM.ToString("0.00")
                            ));
                }
            }
        }

        /// <summary>
        /// Contact the lyrics service
        /// </summary>
        /// <param name="song"></param>
        /// <returns></returns>
        private static Task<LyricsOvh_Reply> GetLyrics(MusicBrainz_Song song)
        {
            //Retrive the lyric
            LyricsOvhProto.LyricsOvhProtoClient client_LyricsOvh = new LyricsOvhProto.LyricsOvhProtoClient(channel);
            LyricsOvh_Request request = new LyricsOvh_Request()
            {
                ArtistName = song.ArtistName,
                SongTitle = song.Title,
            };
            return client_LyricsOvh.GetLyricWithDatabaseHelpAsync(request).ResponseAsync;
        }
    }
}
