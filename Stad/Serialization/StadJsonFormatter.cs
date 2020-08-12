using System;
using System.IO;
using System.Text.Json;

namespace Stad.Serialization
{
    public class StadJsonFormatter : IStadFormatter
    {
        public void Serialize(Stream stream)
        {
        }

        public T Deserialize<T>(ReadOnlySpan<byte> input)
        {
            return JsonSerializer.Deserialize<T>(input);
        }

        public object Deserialize(Type type, ReadOnlySpan<byte> input)
        {
            return JsonSerializer.Deserialize(input, type);
        }
    }
}