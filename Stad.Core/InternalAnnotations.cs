using System.Collections.Immutable;
using Stad.Annotation;

namespace Stad.Core
{
    public class JsonFormatterProxyAttribute : StadAnnotation
    {
        public readonly string Type;
        public JsonFormatterProxyAttribute(ImmutableArray<object> arguments)
        {
            Type = (arguments[0] as System.Type)?.FullName ?? "";
        }
    }
}