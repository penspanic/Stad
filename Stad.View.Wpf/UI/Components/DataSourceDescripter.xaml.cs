using Stad.DataSet;
using Stad.Service;
using System.Windows.Controls;

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
