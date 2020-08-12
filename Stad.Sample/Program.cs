using System;
using System.Threading.Tasks;
using Stad;
using Stad.Annotation;
using Stad.Container;
using Stad.DataSet;
using Stad.Serialization;

namespace Stad.Sample
{
    [StadModel]
    public class SampleSingleModel
    {
        public int IntField;
        public int IntProperty { get; set; }
        public int IntPropertyGetterOnly { get; }
    }

    [StadModel]
    public class SampleKeyValueModel
    {
        public string StringValue;
    }

    [RegistryDefinition]
    public class CommonDataSet
    {
        public SampleSingleModel SampleSingleModel;
        public StadKeyValueCollection<SampleKeyValueModel> SampleKeyValueModel;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CommonDataSet commonDataSet = await StadSerializer.LoadDataAsync<CommonDataSet>(DataSetFactory.LocalFile("Common/"));
        }
    }
}