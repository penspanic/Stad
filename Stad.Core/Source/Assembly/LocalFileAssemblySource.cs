using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stad.Core.Source.Assembly
{
    public class LocalFileAssemblySource : LocalFileSource, IAssemblySource
    {
        public LocalFileAssemblySource(string rootPath) : base(rootPath)
        {
            
        }

        public override bool IsAvailable => true;
        public Task<string[]> GetAssemblyTargetFiles()
        {
            return Task.FromResult(Directory.EnumerateFiles(RootPath, "*.dll", SearchOption.AllDirectories).ToArray());
        }

        public Task<System.Reflection.Assembly> LoadAssembly(string name)
        {
            return Task.FromResult(System.Reflection.Assembly.LoadFrom(RootPath + name));
        }
    }
}
