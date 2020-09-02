using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Stad;
using Stad.Analysis;
using Stad.Annotation;
using Stad.Container;
using Stad.DataSet;
using Stad.Serialization;

namespace Stad.Sample
{
    public class SampleSingleModel
    {
        public int IntField;
        public int IntProperty { get; set; }
    }

    public class SampleKeyValueModel
    {
        public string StringValue;
    }

    public class ComplexDataModel
    {
        public TestVector2 VectorField;
        public GenericModel<int> IntGenericField;
    }

    public readonly struct TestVector2
    {
        public readonly int X;
        public readonly int Y;
    }

    public class GenericModel<T>
    {
        public T GenericField;
    }

    [DataSetDefinition]
    public class CommonDataSet
    {
        public SampleSingleModel SampleSingleModel;
        public StadKeyValueCollection<SampleKeyValueModel> SampleKeyValueModel;
        public ComplexDataModel ComplexDataModel;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            StadSettings.Initialize();
            Console.WriteLine("Hello World!");
            CommonDataSet commonDataSet = new CommonDataSet()
            {
                SampleSingleModel = new SampleSingleModel() {IntField = 1, IntProperty = 2},
                SampleKeyValueModel = new StadKeyValueCollection<SampleKeyValueModel>(
                    new Dictionary<ModelKey, SampleKeyValueModel>()
                    {
                        {new ModelKey(1), new SampleKeyValueModel() {StringValue = "1"}},
                        {new ModelKey(2), new SampleKeyValueModel() {StringValue = "2"}},
                        {new ModelKey(3), new SampleKeyValueModel() {StringValue = "3"}}
                    })
            };

            string dic = Utf8Json.JsonSerializer.ToJsonString(new Dictionary<int, string>(){{1, "333"}, {2, "32423"} });

            // load test
            commonDataSet = await StadSerializer.LoadDataAsync<CommonDataSet>(DataSetFactory.LocalFile("Common/"));

            Console.WriteLine($"SampleSingleModel : {commonDataSet.SampleSingleModel}");
            Console.WriteLine($"SampleKeyValueModel : {commonDataSet.SampleKeyValueModel}");

            Console.WriteLine("Assembly analysis - by source analyze");
            var registry = await StadAnalyzer.MakeRegistryFromSource("../../");
            Console.WriteLine(registry);
        }
    }
}