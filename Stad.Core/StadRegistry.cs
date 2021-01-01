using System.Linq;
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
            return string.Join(",", DataSets);
        }

        public static StadRegistry Create(ReadOnlyCollection<DataSetModel> dataSets)
        {
            StadRegistry stadRegistry = new StadRegistry(dataSets);
            return stadRegistry;
        }

        public static StadRegistry CreateFrom(Stad.Model.RegistryProto proto)
        {
            return new StadRegistry(
                new ReadOnlyCollection<DataSetModel>(
                    proto.DataSets.Select(m => DataSetModel.CreateFrom(m)).ToList())
            );
        }

        private StadRegistry(ReadOnlyCollection<DataSetModel> dataSets)
        {
            DataSets = dataSets;
        }

        public ReadOnlyCollection<DataSetModel> DataSets { get; }
    }
}