using System;
using System.Threading.Tasks;

namespace Stad.Core.Source
{
    public interface ISource : IAsyncDisposable
    {
        bool IsAvailable { get; }
        Task<bool> Initialize();
    }
}