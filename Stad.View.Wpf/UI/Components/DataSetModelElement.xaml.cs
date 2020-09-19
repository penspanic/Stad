using Stad.Core.Model;
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
