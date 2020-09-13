using Stad.Core.Source;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stad.View.Wpf
{
    public static class ExceptionHelper
    {
        public static void AssertAviliable(ISource source)
        {
            if (source.IsAvailable == false)
            {
                throw new Exception($"Source not available : {source}");
            }
        }
    }
}
