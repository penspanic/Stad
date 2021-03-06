﻿using Stad.Core.Model;
using Stad.View.Wpf.UI;
using Stad.View.Wpf.UI.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stad.View.Wpf.UI.Windows
{
    public class TabContext
    {
        public TabRenderType Type;
    }

    public enum TabRenderType
    {
        MainView,
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowManager.Register(this);

            foreach (TabItem tabItem in tabControl.Items)
            {
                InitTabItem(tabItem);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            WindowManager.Unregister(this);
        }

        #region Menu
        private void Assembly_Open_Click(object sender, RoutedEventArgs e)
        {
            new AssemblySourceWindow().ShowDialog();
        }

        private void Data_Open_Click(object sender, RoutedEventArgs e)
        {
            new DataSourceWindow().ShowDialog();
        }
        #endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void InitTabItem(TabItem tabItem)
        {
            if (tabItem.DataContext == null)
            {
                tabItem.DataContext = new TabContext();
            }

            var context = tabItem.DataContext as TabContext;
            tabItem.Content = null;
            tabItem.Header = context.Type.ToString();
            switch (context.Type)
            {
                case TabRenderType.MainView:
                    tabItem.Content = new MainViewRenderer();
                    break;
            }

            if (tabItem.Content is IRenderer renderer)
            {
                renderer.SetTabItem(tabItem);
            }
        }

        #region IWindow Implementation
        public bool TryShow(object context)
        {
            if (context is DataSetModel dataSetModel)
            {
                // TODO: show in tab
                return true;
            }

            return false;
        }
        #endregion
    }
}