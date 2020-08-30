using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Stad.Core.Model
{
    public class DataSetModel
    {
        public DataSetModel(ReadOnlyCollection<StadModel> listModels, ReadOnlyCollection<StadModel> singleModels)
        {
            ListModels = listModels;
            SingleModels = singleModels;
        }

        public ReadOnlyCollection<StadModel> ListModels { get; }
        public ReadOnlyCollection<StadModel> SingleModels { get; }
    }
}