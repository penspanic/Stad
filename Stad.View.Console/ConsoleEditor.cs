using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Stad.Analysis;
using Stad.Core;
using Terminal.Gui;

namespace Stad.View.Console
{
    public class ConsoleEditor : Toplevel
    {
        private StadRegistry _stadRegistry;
        private Window _assemblyWindow;

        public ConsoleEditor()
        {
            var menu = new MenuBar(new []
            {
                new MenuBarItem("_Assembly", new []
                {
                    new MenuItem("_Open", "", Assembly_Open),
                    new MenuItem("_Close", "", Assembly_Close),
                }),
                new MenuBarItem("_Data", new []
                {
                    new MenuItem ("_Open", "", () => {}),
                    new MenuItem ("_Close", "", () => {}),
                }),
                new MenuBarItem("_Help", new []
                {
                    new MenuItem ("_Search", "데이터, 어셈블리 검색??", () => {}),
                    new MenuItem ("_About", "", Help_About),
                }),
            });

            Add(menu);
        }


        #region MenuBar Actions

        private async void Assembly_Open()
        {
            var openDialog = new OpenDialog("Open Assembly", "Select Assembly directory")
                {AllowsMultipleSelection = false, CanChooseDirectories = true, CanChooseFiles = false};
            Application.Run(openDialog);
            if (openDialog.FilePaths.Count != 1)
            {
                return;
            }

            string dirPath = openDialog.FilePaths[0];

            var assemblyFiles = System.IO.Directory.EnumerateFiles(dirPath, "*.dll", SearchOption.AllDirectories).ToArray();
            _stadRegistry = await StadAnalyzer.MakeRegistryFromAssembly(assemblyFiles);
            if (_stadRegistry != null)
            {
                _assemblyWindow = new Window("Assembly") {X = 1, Y = 1, Width = Dim.Fill(), Height = Dim.Fill()};
                _assemblyWindow.Add(new Label(0, 0, $"Assembly : {_stadRegistry}"));
                Add(_assemblyWindow);
            }
        }

        private void Assembly_Close()
        {
            
        }

        private void Data_Open()
        {
            
        }

        private void Data_Close()
        {
            
        }

        private void Help_About()
        {
            var dialog = new Dialog("About", 30, 10);
            dialog.Add(new []
            {
                new Label(0, 0, "Stad.View.Console"),
                new Label(0, 1, "Version : 0.0.0")
            });
            Application.Run(dialog);
        }
        #endregion
    }
}