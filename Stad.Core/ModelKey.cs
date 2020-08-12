namespace Stad.Core
{
    public readonly struct ModelKey
    {
        public ModelKey(int intValue)
        {
            IntValue = intValue;
            StringValue = null;
        }

        public ModelKey(string stringValue)
        {
            StringValue = stringValue;
            IntValue = 0;
        }

        public readonly int IntValue;
        public readonly string StringValue;
    }
}