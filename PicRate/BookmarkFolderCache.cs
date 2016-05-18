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
                cache = Deserialize(Decompress(File.ReadAllBytes(FilePath)));
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
            File.WriteAllBytes(FilePath, Compress(Serialize(cache)));
        }

        private static byte[] Compress(byte[] toCompress)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                    gzip.Write(toCompress, 0, toCompress.Length);

                return ms.ToArray();
            }
        }

        private static byte[] Decompress(byte[] toDecompress)
        {
            using (var gzip = new GZipStream(new MemoryStream(toDecompress), CompressionMode.Decompress))
            {
                const int size = 4096;
                var buffer = new byte[size];
                using (var ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = gzip.Read(buffer, 0, size);
                        if (count > 0)
                            ms.Write(buffer, 0, count);
                    }
                    while (count > 0);
                    return ms.ToArray();
                }
            }
        }

        private static byte[] Serialize(Dictionary<string, BookmarkFolder> toSerialize)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, toSerialize);
                return ms.ToArray();
            }
        }

        private static Dictionary<string, BookmarkFolder> Deserialize(byte[] toDeserialize)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(toDeserialize, 0, toDeserialize.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (Dictionary<string, BookmarkFolder>)bf.Deserialize(ms);
            }
        }
    }
}
