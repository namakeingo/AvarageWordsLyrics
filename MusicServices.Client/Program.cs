using System;
using Grpc.Core;

using MusicBrainz;
using LyricsOvh;

namespace MusicServices.Cliant
{
    class Program
    {
        private static Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);

        /// <summary>
        /// Start of Console App
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);
            //client_LyricsOvh = new LyricsOvhProto.LyricsOvhProtoClient(channel);

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
                //will go back to start of outer while loop
            }
            else if (artistName.Length < 3)
            {
                Console.WriteLine("The artist name should be at least 3 characters long");
                //will go back to start of outer while loop
            }
            else
            {
                MusicBrainz_SearchArtist_Request saRequst = new MusicBrainz_SearchArtist_Request()
                {
                    ArtistName = artistName
                };

                var client_MusicBrainz = new MusicBrainzProto.MusicBrainzProtoClient(channel);
                MusicBrainz_SearchArtist_Reply reply = client_MusicBrainz.SearchArtist(saRequst);

                if (reply.HasError)
                {
                    Console.WriteLine("Sorry there was a problem comunicating with MusicBrainz");
                    Console.WriteLine(reply.ErrorMessage);
                    //will go back to start of outer while loop
                }
                else
                {
                    switch (reply.Artists.Count)
                    {
                        case 0:
                            Console.WriteLine(
                                String.Format("Sorry it looks like there is no artist in MusicBrainz for: \"{0}\"", artistName));
                            //will go back to start of outer while loop
                            break;
                        case 1:
                            //Procees to next section
                            OneArtistSelected(reply.Artists[0]);
                            break;
                        case <= 50:
                            for (int i = 0; i < reply.Artists.Count; i++)
                            {
                                Console.WriteLine(
                                String.Format("{0} - artist:\"{1}\" - https://musicbrainz.org/artist/{2}", i, reply.Artists[i].Name, reply.Artists[i].ID));
                            }
                            //Procees to next section to let the user select form multiple artists
                            SelectFromMultipleArtists(reply);
                            break;
                        case > 50:
                            Console.WriteLine(
                                String.Format("There are {0} results in MusicBrainz for: \"{1}\"", reply.Artists.Count, artistName));
                            Console.WriteLine("Please try to be more specific");
                            //will go back to start of outer while loop
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
            int selectedIndex = 0;

            line = Console.ReadLine();
            if (line != null && line != "")
            {
                if (line.ToLower() == "start")
                {
                    //will go back to start of outer while loop
                }
                else if (Int32.TryParse(line, out selectedIndex) && selectedIndex < reply.Artists.Count)
                {
                    OneArtistSelected(reply.Artists[selectedIndex], selectedIndex);
                }
                else
                {
                    Console.WriteLine(String.Format("\"{0}\" is not a valid input", line));
                    SelectFromMultipleArtists(reply);
                }
            }
            else
            {
                Console.WriteLine("Input cannot be empty");
                SelectFromMultipleArtists(reply);
            }
        }

        private static void OneArtistSelected(MusicBrainz_Artist artist, int? index = null)
        {
            Console.WriteLine(
                        String.Format("Selected: {0}artist:\"{1}\" - https://musicbrainz.org/artist/{2}",
                        index.HasValue ? string.Format("{0} - ", index.Value) : "",
                        artist.Name,
                        artist.ID));
            Console.WriteLine("Looking for songs. Please wait...");

            MusicBrainz_SearchArtistSongs_Request saRequst = new MusicBrainz_SearchArtistSongs_Request()
            {
                ArtistID = artist.ID,
                ArtistName = artist.Name
            };

            var client_MusicBrainz = new MusicBrainzProto.MusicBrainzProtoClient(channel);
            MusicBrainz_SearchArtistSongs_Reply reply = client_MusicBrainz.SearchArtistSongs(saRequst);

            if (reply.HasError)
            {
                Console.WriteLine("Sorry there was a problem comunicating with MusicBrainz");
                Console.WriteLine(reply.ErrorMessage);
                //will go back to start of outer while loop
            }
            else
            {
                switch (reply.Songs.Count)
                {
                    case 0:
                        Console.WriteLine(
                            String.Format("Sorry it looks like MusicBrainz has no songs for: \"{0}\"", artist.Name));
                        //will go back to start of outer while loop
                        break;
                    case 1:
                        Console.WriteLine(
                            String.Format("\n\"{0}\" has only one song titled \"{1}\".", artist.Name, reply.Songs[0].Title));
                        break;
                    case > 1:
                        Console.WriteLine(
                            String.Format("\n\"{0}\" has a total of {1} song.", artist.Name, reply.Songs.Count));
                        break;
                }
            }
        }
    }
}
