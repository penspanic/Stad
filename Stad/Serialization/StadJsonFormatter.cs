using System;
using System.Buffers;
using System.IO;
using Utf8Json;

namespace Stad.Serialization
{
    public class StadJsonFormatter : IStadFormatter
    {
        public void Serialize<T>(T target, Stream stream)
        {
            JsonSerializer.Serialize(stream, target);
        }

        public T Deserialize<T>(ArraySegment<byte> input)
        {
            byte[] rent = ArrayPool<byte>.Shared.Rent(input.Count);
            try
            {
                input.CopyTo(rent);
                return JsonSerializer.Deserialize<T>(rent);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }
    }
}