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
    /// Interaction logic for DataSourceDescripter.xaml
    /// </summary>
    public partial class DataSourceDescripter : UserControl
    {
        public DataSourceDescripter()
        {
            InitializeComponent();
            StadApplication.StadEvent += OnStadEvent;
        }

        private void OnStadEvent(StadEventType type)
        {
            if (type != StadEventType.DataSourceChanged)
            {
                return;
            }

            var dataSets = StadApplication.DataSource.GetDataSets();
            stackpanel_DataSetElement.Children.Clear();

            foreach (IDataSet dataSet in dataSets)
            {
                stackpanel_DataSetElement.Children.Add(new DataSetElement(dataSet));
            }
        }
    }
}
