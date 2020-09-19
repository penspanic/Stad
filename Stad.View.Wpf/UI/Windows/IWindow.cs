using System;
using System.Collections.Generic;
using System.Text;

namespace Stad.View.Wpf.UI.Windows
{
    public interface IWindow
    {
        bool TryShow(object context);
    }
}