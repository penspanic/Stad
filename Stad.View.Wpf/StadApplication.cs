using Stad.Analysis;
using Stad.Core;
using Stad.Core.Source.Assembly;
using Stad.Core.Source.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stad.View.Wpf
{
    public enum StadEventType
    {
        Undefined = 0,
        DataSourceChanged,
        AssemblySourceChanged,
        StadRegistryChanged,
    }

    // TODO: 멀티쓰레딩 고려 필요.
    public static class StadApplication
    {
        public static IDataSource DataSource { get; private set; }
        public static IAssemblySource AssemblySource { get; private set; }
        public static StadRegistry StadRegistry { get; private set; }

        public static event Action<StadEventType> StadEvent
        {
            add
            {
                _stadEventRaw += value;
            }
            remove
            {
                _stadEventRaw -= value;
            }
        }

        private static object _eventLock = new object();
        private static event Action<StadEventType> _stadEventRaw;

        private static void RaiseEvent(StadEventType type)
        {
            lock (_eventLock)
            {
                _stadEventRaw?.Invoke(type);
            }
        }

        public static void SetDataSource(IDataSource dataSource)
        {
            ExceptionHelper.AssertAviliable(dataSource);
            DataSource = dataSource;
            RaiseEvent(StadEventType.DataSourceChanged);
        }

        public static void SetAssemblySource(IAssemblySource assemblySource)
        {
            ExceptionHelper.AssertAviliable(assemblySource);
            AssemblySource = assemblySource;
            RaiseEvent(StadEventType.AssemblySourceChanged);
        }

        public static void SetStadRegitry(StadRegistry stadRegistry)
        {
            StadRegistry = stadRegistry;
            RaiseEvent(StadEventType.StadRegistryChanged);
        }

        public static void Show(object context)
        {

        }
    }
}
