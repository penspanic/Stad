using System.Collections.Generic;
using System.Linq;
using Utf8Json;

namespace Stad.Container
{
    // TODO: 더 적합한 Collection 찾기.
    [JsonFormatter(typeof(StadKeyValueCollectionJsonFormatter<>))]
    public class StadKeyValueCollection<TValue>
    {
        private Dictionary<ModelKey, TValue> _dictionary;
        public StadKeyValueCollection()
        {
            _dictionary = new Dictionary<ModelKey, TValue>();
        }

        public StadKeyValueCollection(IEnumerable<KeyValuePair<ModelKey, TValue>> collection)
        {
            _dictionary = new Dictionary<ModelKey, TValue>(collection);
        }

        public StadKeyValueCollection(Dictionary<ModelKey, TValue> dictionary)
        {
            _dictionary = new Dictionary<ModelKey, TValue>(dictionary);
        }

        public Dictionary<ModelKey, TValue> GetRaw() => _dictionary;

        public bool ContainsKey(ModelKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(ModelKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public TValue this[ModelKey key] => _dictionary[key];

        public Dictionary<ModelKey, TValue>.KeyCollection Keys => _dictionary.Keys;

        public Dictionary<ModelKey, TValue>.ValueCollection Values => _dictionary.Values;

        public Dictionary<ModelKey, TValue>.Enumerator GetEnumerator() => _dictionary.GetEnumerator();
    }

    public class StadKeyValueCollectionJsonFormatter<T> : IJsonFormatter<StadKeyValueCollection<T>>
    {
        public void Serialize(ref JsonWriter writer, StadKeyValueCollection<T> value, IJsonFormatterResolver formatterResolver)
        {
            if (value == null) { writer.WriteNull(); return; }

            // if target type is primitive, you can also use writer.Write***.
            formatterResolver.GetFormatterWithVerify<Dictionary<ModelKey, T>>().Serialize(ref writer, value.GetRaw(), formatterResolver);
        }

        public StadKeyValueCollection<T> Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull()) return null;
            
            // if target type is primitive, you can also use reader.Read***.
            var dictionary = formatterResolver.GetFormatterWithVerify<Dictionary<ModelKey, T>>().Deserialize(ref reader, formatterResolver);

            return new StadKeyValueCollection<T>(dictionary);
        }
    }
}