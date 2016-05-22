using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicRate
{
    abstract class CacheBase<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> cache;
        public readonly string FilePath;

        public CacheBase(string filePath)
        {
            FilePath = filePath;

            try
            {
                cache = CacheHelper.Deserialize<Dictionary<TKey, TValue>>(CacheHelper.Decompress(File.ReadAllBytes(FilePath)));
            }
            catch
            {
                cache = new Dictionary<TKey, TValue>();
            }
        }

        public void Save() => File.WriteAllBytes(FilePath, CacheHelper.Compress(CacheHelper.Serialize(cache)));

        public ICollection<TKey> Keys => cache.Keys;

        public ICollection<TValue> Values => cache.Values;

        public int Count => cache.Count;

        public bool IsReadOnly => false;

        public TValue this[TKey key] { get { return cache[key]; } set { cache[key] = value; } }

        public bool ContainsKey(TKey key) => cache.ContainsKey(key);

        public void Add(TKey key, TValue value) => cache.Add(key, value);

        public bool Remove(TKey key) => cache.Remove(key);

        public bool TryGetValue(TKey key, out TValue value) => cache.TryGetValue(key, out value);

        public void Add(KeyValuePair<TKey, TValue> item) => cache.Add(item.Key, item.Value);

        public void Clear() => cache.Clear();

        public bool Contains(KeyValuePair<TKey, TValue> item) => cache.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)cache).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)cache).Remove(item);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => cache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
