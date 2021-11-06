using System;
using System.Threading.Tasks;
using Grpc.Core;

using LyricsOvh;
using MusicBrainz;
using MusicServices.Services;

namespace MusicServices
{
    class Program
    {
        const int Port = 30051;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { MusicBrainzService.BindService(new MusicBrainzImpl()), LyricsOvhService.BindService(new LyricsOvhImpl()) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RPC Server listening locally on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
