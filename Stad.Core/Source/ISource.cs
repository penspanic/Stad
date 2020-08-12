using System;

namespace Stad.Core.Source
{
    public interface ISource : IAsyncDisposable
    {
        bool IsAvailable { get; }
    }
}