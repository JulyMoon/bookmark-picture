using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PicRate
{
    public static class ExtensionMethods
    {
        public static int Count(this string self, string substring, int startIndex)
        {
            int count = 0;
            for (int i = startIndex; i + substring.Length <= self.Length; i++)
                if (self.Substring(i, substring.Length) == substring)
                    count++;

            return count;
        }

        public static int Count(this string self, string substring) => self.Count(substring, 0);
    }
}
