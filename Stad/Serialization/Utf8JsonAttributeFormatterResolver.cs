using System;
using System.Reflection;
using Utf8Json;

namespace Stad.Serialization
{
    /// <summary>Get formatter from [JsonFormatter] attribute.</summary>
    public sealed class Utf8JsonAttributeFormatterResolver : IJsonFormatterResolver
    {
        public static IJsonFormatterResolver Instance = (IJsonFormatterResolver) new Utf8JsonAttributeFormatterResolver();

        private Utf8JsonAttributeFormatterResolver()
        {
        }

        public IJsonFormatter<T> GetFormatter<T>() => Utf8JsonAttributeFormatterResolver.FormatterCache<T>.formatter;

        private static class FormatterCache<T>
        {
            public static readonly IJsonFormatter<T> formatter;

            static FormatterCache()
            {
                Type tType = typeof(T);
                JsonFormatterAttribute customAttribute = tType.GetTypeInfo().GetCustomAttribute<JsonFormatterAttribute>();
                if (customAttribute == null)
                    return;
                try
                {
                    if (tType.IsGenericType)
                    {
                        Utf8JsonAttributeFormatterResolver.FormatterCache<T>.formatter =
                            (IJsonFormatter<T>) Activator.CreateInstance(
                                customAttribute.FormatterType.MakeGenericType(tType.GenericTypeArguments));
                    }
                    else
                    {
                        if (customAttribute.FormatterType.IsGenericType)
                        {
                            Utf8JsonAttributeFormatterResolver.FormatterCache<T>.formatter =
                                (IJsonFormatter<T>) Activator.CreateInstance(
                                    customAttribute.FormatterType.MakeGenericType(typeof(T)));
                        }
                        else
                        {
                            Utf8JsonAttributeFormatterResolver.FormatterCache<T>.formatter =
                                (IJsonFormatter<T>) Activator.CreateInstance(
                                    customAttribute.FormatterType
                                );
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Can not create formatter from JsonFormatterAttribute, check the target formatter is public and has constructor with right argument. FormatterType:" + customAttribute.FormatterType.Name, ex);
                }
            }
        }
    }
}