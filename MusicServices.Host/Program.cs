using System;
using Grpc.Core;

using LyricsOvh;
using MusicBrainz;
using MusicServices.Services.LyricsOvh;
using MusicServices.Services.MusicBrainz;

namespace MusicServices.Host
{
    class Program
    {
        const int Port = 30052;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { MusicBrainzProto.BindService(new MusicBrainzService()), LyricsOvhProto.BindService(new LyricsOvhService()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RPC Server listening locally on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            LyricsOvhService.Database.Dispose();

            server.ShutdownAsync().Wait();
        }
    }
}
