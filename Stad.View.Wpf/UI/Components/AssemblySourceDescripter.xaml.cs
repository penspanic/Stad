using Stad.Service;
using Stad.Core.Model;
using System.Windows.Controls;

namespace Stad.View.Wpf.UI.Components
{
    /// <summary>
    /// Interaction logic for AssemblySourceDescripter.xaml
    /// </summary>
    public partial class AssemblySourceDescripter : UserControl
    {
        public AssemblySourceDescripter()
        {
            InitializeComponent();
            StadApplication.StadEvent += OnStadEvent;
        }

        private void OnStadEvent(StadEventType type)
        {
            if (type != StadEventType.StadRegistryChanged)
            {
                return;
            }

            var stadRegistry = StadApplication.StadRegistry;
            stackpanel_DataSetElement.Children.Clear();
            if (stadRegistry == null)
            {
                return;
            }

            label_Title.Content = stadRegistry.ToString();

            foreach (DataSetModel model in stadRegistry.DataSets)
            {
                stackpanel_DataSetElement.Children.Add(new DataSetModelElement(model));
            }
        }
    }
}
