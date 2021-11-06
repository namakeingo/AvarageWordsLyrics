using System;
using Grpc.Core;

using MusicBrainz;
using LyricsOvh;

namespace MusicServices.Cliant
{
    class Program
    {
        static void Main(string[] args)
        {
            Channel channel = new Channel("127.0.0.1:30051", ChannelCredentials.Insecure);

            var client_MusicBrainz = new MusicBrainzProto.MusicBrainzProtoClient(channel);
            var client_LyricsOvh = new LyricsOvhProto.LyricsOvhProtoClient(channel);

            channel.ShutdownAsync().Wait();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
