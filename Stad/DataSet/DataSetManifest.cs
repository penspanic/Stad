using System;
using System.Collections.Generic;

namespace Stad.DataSet
{
    public class DataSetManifest
    {
        public List<DataSetItem> Items;

        public static DataSetManifest Deserialize(string jsonStr)
        {
            return Utf8Json.JsonSerializer.Deserialize<DataSetManifest>(jsonStr);
        }
    }
}