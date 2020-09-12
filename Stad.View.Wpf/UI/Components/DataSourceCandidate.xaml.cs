using Microsoft.Win32;
using Stad.View.Wpf.Types;
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

namespace Stad.View.Wpf.UI.Components
{
    /// <summary>
    /// Interaction logic for DataSourceCandidate.xaml
    /// </summary>
    public partial class DataSourceCandidate : UserControl
    {
        public DataSourceType DataSourceType { get; }

        public DataSourceCandidate()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataSourceType == DataSourceType.LocalFile)
            {
                var dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                string name = dialog.FileName;
            }
        }
    }
}
