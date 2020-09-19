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
