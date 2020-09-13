using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Stad.DataSet;

namespace Stad.Core.Source.Data
{
    public class LocalFileDataSource : LocalFileSource, IDataSource
    {
        public LocalFileDataSource(string rootPath) : base(rootPath)
        {
            
        }

        public override bool IsAvailable => _isAvailable;
        private bool _isAvailable;
        List<IDataSet> _dataSets = new List<IDataSet>();

        public override Task<bool> Initialize()
        {
            foreach (string directory in System.IO.Directory.EnumerateDirectories(RootPath))
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(directory, Constants.DataSetManifestPath)) == false)
                {
                    continue;
                }

                var dataSet = new FileSystemDataSet(directory);
                _dataSets.Add(dataSet);
            }

            if (_dataSets.Count == 0)
            {
                return Task.FromResult(false);
            }
 
            _isAvailable = true;
            return Task.FromResult(true);
        }

        public ImmutableArray<IDataSet> GetDataSets()
        {
            return ImmutableArray.Create(_dataSets.ToArray());
        }
    }
}