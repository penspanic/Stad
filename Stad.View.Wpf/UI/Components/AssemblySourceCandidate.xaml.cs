using Stad.Core.Source.Assembly;
using Stad.Core.Source.Data;
using Stad.View.Wpf.Types;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Stad.View.Wpf.UI.Components
{
    /// <summary>
    /// Interaction logic for AssemblySourceCandidate.xaml
    /// </summary>
    public partial class AssemblySourceCandidate : UserControl
    {
        public DataSourceType DataSourceType { get; }

        public AssemblySourceCandidate()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataSourceType == DataSourceType.LocalFile)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                try
                {
                    var assemblySource = new LocalFileAssemblySource(dialog.SelectedPath);
                    if (await assemblySource.Initialize() == false)
                    {
                        MessageBox.Show("Assembly initialize failed!");
                        return;
                    }

                    StadApplication.SetAssemblySource(assemblySource);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
    }
}
