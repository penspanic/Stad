using System.Linq;
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

        public static DataSetModel CreateFrom(Stad.Model.DataSetProto proto)
        {
            return new DataSetModel(
                proto.Name,
                new ReadOnlyCollection<StadModel>(
                    proto.ListModels.Select(m => StadModel.CreateFrom(m)).ToList()),
                new ReadOnlyCollection<StadModel>(
                    proto.SingleModels.Select(m => StadModel.CreateFrom(m)).ToList())
            );
        }

        public string Name { get; }
        public ReadOnlyCollection<StadModel> ListModels { get; }
        public ReadOnlyCollection<StadModel> SingleModels { get; }
    }
}