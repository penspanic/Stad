namespace Stad.Core
{
    public static class Utility
    {
        public static int ToTypeCode(string value)
        {
            int hash = 0;
            for (int i = 0; i < value.Length; ++i)
            {
                hash += value[i];
                hash += hash << 10;
                hash ^= hash >> 6;
            }
            
            hash += hash << 3;
            hash ^= hash >> 11;
            hash += hash << 15;

            return hash;
        }
    }
}