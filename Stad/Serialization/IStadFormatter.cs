using System;
using System.IO;

namespace Stad.Serialization
{
    public interface IStadFormatter
    {
        public void Serialize<T>(T target, Stream stream);
        public T Deserialize<T>(ArraySegment<byte> input);
    }
}