using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Stad.DataSet
{
    public interface IDataSet
    {
        public string Name { get; }
        public Task<object> LoadAsync(Type type, DataSetItem item);
        public Task<List<DataSetItem>> GetItems();
    }
}