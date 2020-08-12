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
        private readonly string _rootPath;

        public FileSystemDataSet(string rootPath)
        {
            _rootPath = rootPath;
        }

        public Task<object> LoadAsync(Type type, DataSetItem item)
        {
            var formatter = CreateFormatter(item);
            var content = ReadContent(item);
            return Task.FromResult(formatter.Deserialize(type, content));
        }

        public Task<List<DataSetItem>> GetItems()
        {
            string path = _rootPath + Constants.ManifestPath;
            if (File.Exists(path) == false)
            {
                return null;
            }

            var manifest = JsonSerializer.Deserialize<DataSetManifest>(File.ReadAllBytes(path));
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

        private ReadOnlySpan<byte> ReadContent(DataSetItem item)
        {
            return File.ReadAllBytes(_rootPath + item.Name);
        }
    }
}