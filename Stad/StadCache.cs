using System;
using System.Collections.Generic;

namespace Stad
{
    /// <summary>
    /// Thread not safe
    /// </summary>
    public static class StadCache
    {
        private static Dictionary<Type, object> _dictionary = new Dictionary<Type, object>();
        public static void Set<T>(T dataSet)
        {
            _dictionary.Add(typeof(T), dataSet);
        }

        public static T Get<T>()
        {
            _dictionary.TryGetValue(typeof(T), out object dataSetRaw);
            return (T) dataSetRaw;
        }

        public static void Clear()
        {
            _dictionary.Clear();
        }
    }
}