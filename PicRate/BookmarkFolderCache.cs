using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace PicRate
{
    class BookmarkFolderCache
    {
        private Dictionary<string, BookmarkFolder> cache;
        private MD5 md5 = MD5.Create();
        public readonly string FilePath;

        public BookmarkFolderCache(string filePath)
        {
            FilePath = filePath;

            try
            {
                cache = CacheHelper.Deserialize<Dictionary<string, BookmarkFolder>>(CacheHelper.Decompress(File.ReadAllBytes(FilePath)));
            }
            catch
            {
                cache = new Dictionary<string, BookmarkFolder>();
            }
        }

        public BookmarkFolder Add(byte[] serialized)
        {
            var md5hash = Convert.ToBase64String(md5.ComputeHash(serialized));

            if (!cache.ContainsKey(md5hash))
                cache.Add(md5hash, HTMLBookmarkParser.Parse(Encoding.Default.GetString(serialized)));

            return cache[md5hash];
        }

        public void Save()
        {
            File.WriteAllBytes(FilePath, CacheHelper.Compress(CacheHelper.Serialize(cache)));
        }
    }
}
