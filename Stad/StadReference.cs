namespace Stad
{
    public readonly struct StadReference
    {
        public StadReference(ModelKey key)
        {
            Key = key;
        }

        public readonly ModelKey Key;

        public T Get<T>()
        {
            // TODO: implement
            return default;
        }
    }
}