using System.Threading.Tasks;
using Stad.DataSet;

namespace Stad.Serialization
{
    public static class StadSerializer
    {
        public static async Task<T> LoadDataAsync<T>(IDataSet dataSet) where T : new()
        {
            if (dataSet == null)
            {
                return default;
            }

            var items = await dataSet.GetItems();
            if (items == null)
            {
                return default;
            }

            T result = new T();
            foreach (DataSetItem item in items)
            {
                var memberInfo = ReflectionUtility.FindMember(result, item);
                object value = dataSet.LoadAsync(memberInfo.DeclaringType, item);
                ReflectionUtility.SetValue(result, item, value);
            }

            return result;
        }
    }
}