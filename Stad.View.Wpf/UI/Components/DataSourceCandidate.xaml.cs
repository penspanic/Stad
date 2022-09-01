using Stad.Core.Source.Data;
using Stad.Service;
using Stad.View.Wpf.Types;
using System;
using System.Windows;
using System.Windows.Controls;

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataSourceType == DataSourceType.LocalFile)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                try
                {
                    var dataSource = new LocalFileDataSource(dialog.SelectedPath);
                    if (await dataSource.Initialize() == false)
                    {
                        MessageBox.Show("DataSource initialize failed!");
                        return;
                    }

                    StadApplication.SetDataSource(dataSource);
                    Window.GetWindow(this).Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
    }
}
