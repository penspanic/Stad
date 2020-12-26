using System;
using Stad.Core.Source;

namespace Stad.Client
{
    public static class ExceptionHelper
    {
        public static void AssertAvailable(ISource source)
        {
            if (source.IsAvailable == false)
            {
                throw new Exception($"Source not available : {source}");
            }
        }
    }
}