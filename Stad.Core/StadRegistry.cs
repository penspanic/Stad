using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Stad.Core.Model;

namespace Stad.Core
{
    public readonly struct Path
    {
        public Path(string value)
        {
            Value = value;
        }

        public readonly string Value;
    }

    /// <summary>
    /// 
    /// </summary>
    public class StadRegistry
    {
        public override string ToString()
        {
            return base.ToString();
        }

        public static StadRegistry Create(ReadOnlyCollection<DataSetModel> dataSets)
        {
            StadRegistry stadRegistry = new StadRegistry(dataSets);
            return stadRegistry;
        }

        private StadRegistry(ReadOnlyCollection<DataSetModel> dataSets)
        {
            DataSets = dataSets;
        }

        public ReadOnlyCollection<DataSetModel> DataSets { get; }
    }
}