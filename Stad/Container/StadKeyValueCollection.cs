using System.Collections.Generic;
using System.Linq;

namespace Stad.Container
{
    // TODO: 더 적합한 Collection 찾기.
    public class StadKeyValueCollection<TValue>
    {
        private Dictionary<ModelKey, TValue> _dictionary = new Dictionary<ModelKey, TValue>();
        public StadKeyValueCollection()
        {
            
        }

        public bool ContainsKey(ModelKey key) => _dictionary.ContainsKey(key);

        public bool TryGetValue(ModelKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public TValue this[ModelKey key] => _dictionary[key];

        public Dictionary<ModelKey, TValue>.KeyCollection Keys => _dictionary.Keys;

        public Dictionary<ModelKey, TValue>.ValueCollection Values => _dictionary.Values;

        public Dictionary<ModelKey, TValue>.Enumerator GetEnumerator() => _dictionary.GetEnumerator();
    }
}