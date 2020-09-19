using Stad.View.Wpf.UI.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Windows;

namespace Stad.View.Wpf
{
    public static class WindowManager
    {
        private static ImmutableList<Window> _windows = ImmutableList.Create<Window>();
        private static object _collectionLock = new object();
        public static void Register(Window window)
        {
            lock (_collectionLock)
            {
                var list = Volatile.Read<ImmutableList<Window>>(ref _windows);
                if (list.Contains(window) == true)
                {
                    throw new Exception($"already registered : {window}");
                }

                Volatile.Write(ref _windows, list.Add(window));
            }
        }

        public static void Unregister(Window window)
        {
            lock (_collectionLock)
            {
                var list = Volatile.Read<ImmutableList<Window>>(ref _windows);
                Volatile.Write(ref _windows, list.Remove(window));
            }
        }

        public static bool TryShow(object context)
        {
            var list = Volatile.Read(ref _windows);
            foreach (Window window in list)
            {
                if (window is IWindow iWindow)
                {
                    if (iWindow.TryShow(context) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
