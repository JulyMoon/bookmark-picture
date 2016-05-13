using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PicRate
{
    class BookmarkCollection
    {
        private List<BookmarkBase> collection;
        private const string folderContentStartTag = "<DL>";
        private const string folderContentEndTag = "</DL>";
        private const string bookmarkBase = "<DT>";
        private const string bookmarkStartTag = "<A";
        private const string bookmarkEndTag = "</A>";
        private const string folderDescriptionStartTag = "<H3";
        private const string folderDescriptionEndTag = "</H3>";
        private static readonly Regex folderDescriptionRegex = new Regex(@"^<H3 ADD_DATE=\""(?<addDate>\d+)\"" LAST_MODIFIED=\""(?<lastModified>\d+)\""( PERSONAL_TOOLBAR_FOLDER=\""true\"")?>(?<title>.*)<\/H3>$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex bookmarkRegex = new Regex(@"^<A HREF=\""(?<link>.*)\"" ADD_DATE=\""(?<addDate>\d+)\""( ICON=\""data:image/png;base64,(?<base64string>.+)\"")?>(?<title>.*)</A>$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        //private static readonly Regex bookmarkRegex = new Regex(@".{9}(?<link>.*)\"".{10}\""(?<addDate>\d+)\"".{29}(?<base64string>.+)\"".(?<title>.*)<.{3}", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public BookmarkBase this[int number] => collection[number];

        public BookmarkCollection(string filePath)
        {
            string contents = File.ReadAllText(filePath).Replace("<p>", "");

            int start, length;
            GetFolderContentBounds(contents, 0, out start, out length);

            int currentIndex = 0, currentDepth = 0; // not sure if currentDepth is needed
            collection = Parse(contents.Substring(start, length), ref currentIndex, ref currentDepth);
        }

        private List<BookmarkBase> Parse(string contents, ref int currentIndex, ref int currentDepth)
        {
            var list = new List<BookmarkBase>();

            while (true)
            {
                int bookmarkBaseIndex = contents.IndexOf(bookmarkBase, currentIndex);
                int folderContentEndIndex = contents.IndexOf(folderContentEndTag, currentIndex);

                if (bookmarkBaseIndex == -1 || (folderContentEndIndex != -1 && folderContentEndIndex < bookmarkBaseIndex))
                    break;

                currentIndex = bookmarkBaseIndex + bookmarkBase.Length;
                int folderDescriptionStartIndex = contents.IndexOf(folderDescriptionStartTag, currentIndex);
                int bookmarkStartIndex = contents.IndexOf(bookmarkStartTag, currentIndex);

                if (folderDescriptionStartIndex == currentIndex)
                {
                    int folderDescriptionEndIndex = contents.IndexOf(folderDescriptionEndTag, folderDescriptionStartIndex + folderDescriptionStartTag.Length);
                    if (folderDescriptionEndIndex == -1)
                        throw new ArgumentException("Folder description end tag not found");
                    folderDescriptionEndIndex += folderDescriptionEndTag.Length;

                    string folderDescriptionRaw = contents.Substring(folderDescriptionStartIndex, folderDescriptionEndIndex - folderDescriptionStartIndex);
                    DateTime addDate, lastModified;
                    string title;
                    ParseFolderDescription(folderDescriptionRaw, out addDate, out lastModified, out title);

                    currentIndex = contents.IndexOf(folderContentStartTag, folderDescriptionEndIndex) + folderContentStartTag.Length;
                    currentDepth++;
                    list.Add(new BookmarkFolder(addDate, title, lastModified, Parse(contents, ref currentIndex, ref currentDepth)));
                    currentDepth--;
                    currentIndex = contents.IndexOf(folderContentEndTag, currentIndex) + folderContentEndTag.Length;
                }
                else if (bookmarkStartIndex == currentIndex)
                {
                    int bookmarkEndIndex = contents.IndexOf(bookmarkEndTag, bookmarkStartIndex + bookmarkStartTag.Length);
                    if (bookmarkEndIndex == -1)
                        throw new ArgumentException("Bookmark end tag not found");
                    bookmarkEndIndex += bookmarkEndTag.Length;

                    string bookmarkRaw = contents.Substring(bookmarkStartIndex, bookmarkEndIndex - bookmarkStartIndex);
                    list.Add(ParseBookmark(bookmarkRaw));

                    currentIndex = bookmarkEndIndex;
                }
                else throw new ArgumentException("Unexpected tag after " + bookmarkBase);
            }

            return list;
        }

        private void GetFolderContentBounds(string contents, int startIndex, out int start, out int length)
        {
            int startCount = contents.Count(folderContentStartTag, startIndex);
            int endCount = contents.Count(folderContentEndTag, startIndex);

            if (startCount > endCount)
                throw new ArgumentException("The number of folder content start tags is greater than the number of folder content end tags");

            if (startCount == 0)
                throw new ArgumentException("There are no folder content tags");

            int currentIndex = start = contents.IndexOf(folderContentStartTag, startIndex) + folderContentStartTag.Length;

            int depth = 1;

            while (true)
            {
                int startTagIndex = contents.IndexOf(folderContentStartTag, currentIndex);
                int endTagIndex = contents.IndexOf(folderContentEndTag, currentIndex);

                bool startIsTrueEndIsFalse = startTagIndex == -1 ? false : startTagIndex < endTagIndex;
                if (startIsTrueEndIsFalse)
                {
                    currentIndex = startTagIndex + folderContentStartTag.Length;
                    depth++;
                }
                else
                {
                    if (depth == 1)
                    {
                        length = endTagIndex - start;
                        break;
                    }

                    currentIndex = endTagIndex + folderContentEndTag.Length;
                    depth--;
                }
            }
        }

        private void ParseFolderDescription(string contents, out DateTime addDate, out DateTime lastModified, out string title)
        {
            var match = folderDescriptionRegex.Match(contents);

            if (!match.Success)
                throw new ArgumentException("Invalid folder description");

            addDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Int64.Parse(match.Groups["addDate"].Value)); // not sure if UTC
            lastModified = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Int64.Parse(match.Groups["lastModified"].Value)); // ^ same
            title = match.Groups["title"].Value;
        }

        private Bookmark ParseBookmark(string contents)
        {
            var match = bookmarkRegex.Match(contents);

            if (!match.Success)
                throw new ArgumentException("Invalid bookmark format");

            var addDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Int64.Parse(match.Groups["addDate"].Value)); // not sure if UTC
            var title = match.Groups["title"].Value;
            var link = match.Groups["link"].Value;
            // TODO: match.Groups["base64string"].Value

            return new Bookmark(addDate, title, link);
        }

        private Image GetImageFromBase64String(string base64string)
        {
            var data = Convert.FromBase64String(base64string);
            using (var stream = new MemoryStream(data, 0, data.Length))
                return Image.FromStream(stream);
        }
    }

    class BookmarkFolder : BookmarkBase
    {
        public readonly DateTime LastModified;
        private List<BookmarkBase> collection;

        public BookmarkBase this[int number] => collection[number];

        public BookmarkFolder(DateTime addDate, string title, DateTime lastModified, List<BookmarkBase> bookmarkBases) : base(addDate, title)
        {
            LastModified = lastModified;
            collection = bookmarkBases;
        }
    }

    class Bookmark : BookmarkBase
    {
        public readonly string Link;
        // public Bitmap Icon; TODO

        public Bookmark(DateTime addDate, string title, string link) : base(addDate, title)
        {
            Link = link;
        }
    }

    abstract class BookmarkBase
    {
        public readonly DateTime AddDate;
        public readonly string Title;

        public BookmarkBase(DateTime addDate, string title)
        {
            AddDate = addDate;
            Title = title;
        }
    }

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

        public static int Count(this string self, string substring)
        {
            return self.Count(substring, 0);
        }
    }
}