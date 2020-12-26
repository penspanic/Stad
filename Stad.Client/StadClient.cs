using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Stad.Client
{
    public static class StadClient
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

            while (TerminateToken.IsCancellationRequested == false)
            {
                Thread.Sleep(10);
            }

            await _server.KillAsync();
            _serverTerminateCompletionSource.SetResult();
        }
    }
}