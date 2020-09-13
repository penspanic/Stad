using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stad.Core.Source
{
    public abstract class LocalFileSource : ISource
    {
        public abstract bool IsAvailable { get; }
        public readonly string RootPath;

        public LocalFileSource(string rootPath)
        {
            RootPath = rootPath;
        }

        public virtual Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask(Task.CompletedTask);
;        }
    }
}
