namespace Stad.DataSet
{
    public readonly struct DataSetItem
    {
        public DataSetItem(string name, int sizeBytes, EncodingType encodingType)
        {
            Name = name;
            SizeBytes = sizeBytes;
            EncodingType = encodingType;
        }

        public readonly string Name;
        public readonly int SizeBytes;
        public readonly EncodingType EncodingType;
    }
}