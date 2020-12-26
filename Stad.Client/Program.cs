using System;
using Grpc.Core;

namespace Stad.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = {StadService.BindService(new StadServiceImpl())},
                Ports = {new ServerPort("localhost", 46755, ServerCredentials.Insecure)}
            };

            StadClient.Run(server).Wait();
        }
    }
}