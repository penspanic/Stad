using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace Stad.Service
{
    public class StadServiceImpl : StadService.StadServiceBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply() {Message = "Impl!"});
        }

        public override async Task<TerminateReply> Terminate(TerminateRequest request, ServerCallContext context)
        {
            // TODO: state 세분화
            ServerApplication.TerminateToken.Cancel();
            while (ServerApplication.IsServerTerminated == false)
            {
                Thread.Sleep(10);
            }

            return new TerminateReply();
        }

        public override Task<LoadAssemblyReply> LoadAssemblySource(LoadAssemblyRequest request, ServerCallContext context)
        {
            return base.LoadAssemblySource(request, context);
        }

        public override Task<LoadDataReply> LoadDataSource(LoadDataRequest request, ServerCallContext context)
        {
            return base.LoadDataSource(request, context);
        }
    }
}