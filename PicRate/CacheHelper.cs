using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;

namespace PicRate
{
    static class CacheHelper
    {
        public static byte[] Compress(byte[] toCompress)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                    gzip.Write(toCompress, 0, toCompress.Length);

                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] toDecompress)
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

        public static byte[] Serialize<T>(T toSerialize)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, toSerialize);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] toDeserialize)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                ms.Write(toDeserialize, 0, toDeserialize.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)bf.Deserialize(ms);
            }
        }
    }
}
