using System;
using System.IO;

namespace Stad.Serialization
{
    public interface IStadFormatter
    {
        public void Serialize(Stream stream);
        public T Deserialize<T>(ReadOnlySpan<byte> input);
        public object Deserialize(Type type, ReadOnlySpan<byte> input);
    }
}