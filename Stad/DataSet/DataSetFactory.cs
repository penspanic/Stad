using System;

namespace Stad.DataSet
{
    public static class DataSetFactory
    {
        public static IDataSet LocalFile(string rootPath)
        {
            return new FileSystemDataSet(rootPath);
        }

        public static IDataSet AmazonS3()
        {
            throw new NotImplementedException();
        }
    }
}