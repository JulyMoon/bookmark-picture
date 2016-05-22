using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PicRate
{
    class ImageCache : IDictionary<string, Image>
    {
        private Dictionary<string, Image> cache;
        public readonly string FilePath;

        public ImageCache(string filePath)
        {
            FilePath = filePath;

            try
            {
                cache = CacheHelper.Deserialize<Dictionary<string, Image>>(CacheHelper.Decompress(File.ReadAllBytes(FilePath)));
            }
            catch
            {
                cache = new Dictionary<string, Image>();
            }
        }

        public void Save() => File.WriteAllBytes(FilePath, CacheHelper.Compress(CacheHelper.Serialize(cache)));

        public Image this[string key] { get { return cache[key]; } set { cache[key] = value; } }

        public ICollection<string> Keys => cache.Keys;

        public ICollection<Image> Values => cache.Values;

        public int Count => cache.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(string key) => cache.ContainsKey(key);

        public void Add(string key, Image value) => cache.Add(key, value);

        public bool Remove(string key) => cache.Remove(key);

        public bool TryGetValue(string key, out Image value) => cache.TryGetValue(key, out value);

        public void Add(KeyValuePair<string, Image> item) => ((IDictionary<string, Image>)cache).Add(item);

        public void Clear() => cache.Clear();

        public bool Contains(KeyValuePair<string, Image> item) => cache.Contains(item);

        public void CopyTo(KeyValuePair<string, Image>[] array, int arrayIndex) => ((IDictionary<string, Image>)cache).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, Image> item) => ((IDictionary<string, Image>)cache).Remove(item);

        public IEnumerator<KeyValuePair<string, Image>> GetEnumerator() => cache.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
