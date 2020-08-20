using System.Threading;
using Utf8Json.Formatters;
using Utf8Json.Resolvers;

namespace Stad
{
    public static class StadSettings
    {
        private static long _initStateUnsafe = 0;

        public static void Initialize()
        {
            if (Interlocked.Exchange(ref _initStateUnsafe, 1) == 1)
            {
                return;
            }

            // TODO: UTF8Json 설정이 공유될 수 있음.
            Utf8Json.Resolvers.CompositeResolver.RegisterAndSetAsDefault(
                new[]
                {
                    PrimitiveObjectFormatter.Default,
                },
                new[]
                {
                    Serialization.Utf8JsonAttributeFormatterResolver.Instance, 
                    StandardResolver.Default
                }
            );
        }
    }
}