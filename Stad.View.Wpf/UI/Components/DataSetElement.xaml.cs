using Stad.DataSet;
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
    /// Interaction logic for DataSetElement.xaml
    /// </summary>
    public partial class DataSetElement : UserControl
    {
        public DataSetElement(IDataSet dataSet)
        {
            InitializeComponent();
            label_Header.Content = dataSet.Name;
        }
    }
}
