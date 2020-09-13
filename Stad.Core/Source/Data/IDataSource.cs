using System.Collections.Immutable;
using Stad.DataSet;

namespace Stad.Core.Source.Data
{
    /* [DataSource Structure]
     * Root/DataSet/DataSetItem
     */
    public interface IDataSource : ISource
    {
        public ImmutableArray<IDataSet> GetDataSets();
    }
}