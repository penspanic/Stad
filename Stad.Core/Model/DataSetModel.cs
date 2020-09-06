using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Stad.Core.Model
{
    public class DataSetModel
    {
        public override string ToString()
        {
            return $"[{Name}|List:{ListModels?.Count} Single:{SingleModels?.Count}]";
        }

        public DataSetModel(string name, ReadOnlyCollection<StadModel> listModels, ReadOnlyCollection<StadModel> singleModels)
        {
            Name = name;
            ListModels = listModels;
            SingleModels = singleModels;
        }

        public string Name { get; }
        public ReadOnlyCollection<StadModel> ListModels { get; }
        public ReadOnlyCollection<StadModel> SingleModels { get; }
    }
}