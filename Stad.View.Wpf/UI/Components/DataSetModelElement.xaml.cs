using Stad.Core.Model;
using Stad.Service;
using System.Windows;
using System.Windows.Controls;

namespace Stad.View.Wpf.UI.Components
{
    /// <summary>
    /// Interaction logic for DataSetModelElement.xaml
    /// </summary>
    public partial class DataSetModelElement : UserControl
    {
        public readonly DataSetModel Model;
        public DataSetModelElement(DataSetModel dataSetModel)
        {
            InitializeComponent();
            Model = dataSetModel;

            label_Header.Content = dataSetModel.Name;
            label_Header.Content = dataSetModel.ToString();
        }

        private void button_Detail_Click(object sender, RoutedEventArgs e)
        {
            StadApplication.Show(Model);
        }
    }
}
