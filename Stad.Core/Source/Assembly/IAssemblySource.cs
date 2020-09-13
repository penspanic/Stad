using System.Threading.Tasks;

namespace Stad.Core.Source.Assembly
{
    public interface IAssemblySource : ISource
    {
        public Task<string[]> GetAssemblyTargetFiles();
        public Task<System.Reflection.Assembly> LoadAssembly(string name);
    }
}