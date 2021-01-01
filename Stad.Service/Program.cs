using System;
using Grpc.Core;

namespace Stad.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = {StadService.BindService(new StadServiceImpl())},
                Ports = {new ServerPort("127.0.0.1", 46755, ServerCredentials.Insecure)}
            };

            ServerApplication.Run(server).Wait();
        }
    }
}