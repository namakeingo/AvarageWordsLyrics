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
            ArtistInput();
        }

        /// <summary>
        /// Section of the console app allowing for input of Artist Name
        /// </summary>
        private static void ArtistInput()
        {
            Console.WriteLine("\nPlese type the name of the artist, then press enter to proceed...");
            string artistName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(artistName))
            {
                Console.WriteLine("The artist name should not be Empty or only white spaces");
                ArtistInput();
            }
            else if (artistName.Length < 3)
            {
                Console.WriteLine("The artist name should be at least 3 characters long");
                ArtistInput();
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
                    ArtistInput();
                }
                else
                {
                    switch (reply.Artists.Count)
                    {
                        case 0:
                            Console.WriteLine(
                                String.Format("Sorry it looks like there is no artist in MusicBrainz for: \"{0}\"", artistName));
                            ArtistInput();
                            break;
                        case 1:
                            break;
                        case > 1:
                            for (int i = 0; i < reply.Artists.Count; i++)
                            {
                                Console.WriteLine(
                                String.Format("{0} - artist:\"{1}\" - https://musicbrainz.org/artist/{2}", i, reply.Artists[i].ArtistName, reply.Artists[i].ArtistID));
                            }
                            SelectFromMultipleArtists(ref reply);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Section of the console app allowing for selecting betwen multiple artists found from the search
        /// </summary>
        /// <param name="reply"></param>
        private static void SelectFromMultipleArtists(ref MusicBrainz_SearchArtist_Reply reply)
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
                    ArtistInput();
                }
                else if (Int32.TryParse(line, out selectedIndex) && selectedIndex < reply.Artists.Count)
                {
                    Console.WriteLine(
                        String.Format("Selected: {0} - artist:\"{1}\" - https://musicbrainz.org/artist/{2}", selectedIndex, reply.Artists[selectedIndex].ArtistName, reply.Artists[selectedIndex].ArtistID));
                }
                else
                {
                    Console.WriteLine(String.Format("\"{0}\" is not a valid input",line));
                    SelectFromMultipleArtists(ref reply);
                }
            }
            else
            {
                Console.WriteLine("Input cannot be empty");
                SelectFromMultipleArtists(ref reply);
            }
        }
    }
}
