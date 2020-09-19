using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stad.View.Wpf.UI.Renderers
{
    /// <summary>
    /// Interaction logic for DataSetModelRenderer.xaml
    /// </summary>
    public partial class DataSetModelRenderer : AbstractRenderer, IRenderer
    {
        public TabItem TabItem { get; private set; }
        public DataSetModelRenderer()
        {
            InitializeComponent();
        }

        public void SetTabItem(TabItem tabItem)
        {
            TabItem = tabItem;
        }
    }
}
