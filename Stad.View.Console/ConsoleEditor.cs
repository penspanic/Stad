using System.Threading.Tasks;
using Terminal.Gui;

namespace Stad.View.Console
{
    public class ConsoleEditor : Toplevel
    {
        public ConsoleEditor()
        {
            var menu = new MenuBar(new []
            {
                new MenuBarItem("_File", new []
                {
                    new MenuItem("_Open", "", null),
                    new MenuItem("_Close", "", null),
                }),
                new MenuBarItem("_Edit", new []
                {
                    new MenuItem ("_Copy", "", () => {}),
                    new MenuItem ("C_ut", "", () => {}),
                    new MenuItem ("_Paste", "", () => { })
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