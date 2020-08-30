using System;
using System.Threading.Tasks;
using ConsoleAppFramework;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;

namespace Stad.View.Console
{
    public class Program : ConsoleAppBase
    {
        private static async Task Main(string[] args)
        {
            Application.Run<ConsoleEditor>();
        }

        // private static async Task Main(string[] args)
        // {
        //     await Host.CreateDefaultBuilder()
        //         .ConfigureLogging(logging => logging.ReplaceToSimpleConsole())
        //         .RunConsoleAppFrameworkAsync<Program>(args)
        //         .ConfigureAwait(false);
        // }
        //
        // public async Task RunAsync(
        //     [Option("i", "Input path of analyze csproj or directory, if input multiple csproj split with ','.")]string input,
        //     [Option("o", "Output file path(.cs) or directory(multiple generate file).")]string output,
        //     [Option("c", "Conditional compiler symbols, split with ','.")]string conditionalSymbol = null,
        //     [Option("r", "Set resolver name.")]string resolverName = "GeneratedResolver",
        //     [Option("n", "Set namespace root name.")]string @namespace = "MessagePack",
        //     [Option("m", "Force use map mode serialization.")]bool useMapMode = false,
        //     [Option("ms", "Generate #if-- files by symbols, split with ','.")]string multipleIfDirectiveOutputSymbols = null)
        // {
        //     await new ConsoleEditor().Run().ConfigureAwait(false);
        // }
    }
}