using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Stad.Core;
using Stad.Core.Source.Assembly;

namespace Stad.Analysis
{
    public static class StadAnalyzer
    {
        public static async Task<StadRegistry> MakeRegistry(string rootPath)
        {
            var compilation = await CreateFromDirectoryAsync(rootPath, null, CancellationToken.None);
            TypeCollector typeCollector = new TypeCollector(compilation, true, false, (str) => Console.WriteLine(str));
            var result = typeCollector.Collect();
            return MakeRegistry(result);
        }

        private static Task<CSharpCompilation> CreateFromProjectAsync(string[] csprojs, string[] preprocessorSymbols, CancellationToken cancellationToken)
        {
            return PseudoCompilation.CreateFromProjectAsync(csprojs, preprocessorSymbols, cancellationToken);
        }

        private static Task<CSharpCompilation> CreateFromDirectoryAsync(string directoryRoot, string[] preprocessorSymbols, CancellationToken cancellationToken)
        {
            return PseudoCompilation.CreateFromDirectoryAsync(directoryRoot, preprocessorSymbols, cancellationToken);
        }

        private static StadRegistry MakeRegistry(TypeCollectorResult result)
        {
            var registry = new StadRegistry();

            foreach (ObjectSerializationInfo info in result.CollectedObjectInfo)
            {
            }

            return registry;
        }
    }
}