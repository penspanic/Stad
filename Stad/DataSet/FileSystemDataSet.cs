using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using MessagePack;
using Stad.Serialization;

namespace Stad.DataSet
{
    public class FileSystemDataSet : IDataSet
    {
        public string Name { get; }
        private readonly string _rootPath;

        public FileSystemDataSet(string rootPath)
        {
            _rootPath = rootPath;
            Name = Path.GetFileName(rootPath);
        }

        public Task<object> LoadAsync(Type type, DataSetItem item)
        {
            var formatter = CreateFormatter(item);
            var content = ReadContent(item);
            return Task.FromResult(CallGenericDeserialize(type, formatter, content));
        }

        private object CallGenericDeserialize(Type type, IStadFormatter formatter, ArraySegment<byte> content)
        {
            // TODO: 최적화
            Func<ArraySegment<byte>, object> method = (Func<ArraySegment<byte>, object>) typeof(IStadFormatter)
                .GetMethod("Deserialize").MakeGenericMethod(type)
                .CreateDelegate(typeof(Func<ArraySegment<byte>, object>), formatter);

            return method(content);
        }

        public Task<List<DataSetItem>> GetItems()
        {
            string path = _rootPath + Constants.DataSetManifestPath;
            if (File.Exists(path) == false)
            {
                return null;
            }

            var manifest = DataSetManifest.Deserialize(File.ReadAllText(path));
            return Task.FromResult(manifest.Items);
        }

        private IStadFormatter CreateFormatter(DataSetItem item)
        {
            switch (item.EncodingType)
            {
                case EncodingType.MessagePack:
                    break;
                case EncodingType.Json:
                    return new StadJsonFormatter();
                    break;
            }

            return null;
        }

        private ArraySegment<byte> ReadContent(DataSetItem item)
        {
            // TODO: stackalloc된 array에 copy 하도록 하여 return?
            return File.ReadAllBytes($"{_rootPath + item.Name}.{item.EncodingType}");
        }
    }
}