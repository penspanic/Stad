using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Stad.Service
{
    public static class ServerApplication
    {
        public static CancellationTokenSource TerminateToken { get; private set; }
        public static bool IsServerTerminated => _serverTerminateCompletionSource?.Task.IsCompleted ?? false;

        private static Server _server;
        private static TaskCompletionSource _serverTerminateCompletionSource; 

        public static async Task Run(Server server)
        {
            _server = server;
            _server.Start();

            Console.WriteLine("Server started!");

            TerminateToken = new CancellationTokenSource();
            _serverTerminateCompletionSource = new TaskCompletionSource();

            // server test self
            using var channel = Grpc.Net.Client.GrpcChannel.ForAddress("http://localhost:46755");
            var client = new StadService.StadServiceClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("test success!");
            //

            while (TerminateToken.IsCancellationRequested == false)
            {
                Thread.Sleep(10);
            }

            await _server.KillAsync();
            _serverTerminateCompletionSource.SetResult();
        }
    }
}