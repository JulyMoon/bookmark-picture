using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PicRate
{
    static class HTMLBookmarkParser
    {
        private const string folderContentStartTag = "<DL>";
        private const string folderContentEndTag = "</DL>";
        private const string bookmarkBase = "<DT>";
        private const string bookmarkStartTag = "<A";
        private const string bookmarkEndTag = "</A>";
        private const string folderDescriptionStartTag = "<H3";
        private const string folderDescriptionEndTag = "</H3>";
        private static readonly Regex folderDescriptionRegex = new Regex(@"^<H3 ADD_DATE=\""(?<addDate>\d+)\"" LAST_MODIFIED=\""(?<lastModified>\d+)\""( PERSONAL_TOOLBAR_FOLDER=\""true\"")?>(?<title>.*)<\/H3>$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
        private static readonly Regex bookmarkRegex = new Regex(@"^<A HREF=\""(?<link>.*)\"" ADD_DATE=\""(?<addDate>\d+)\""( ICON=\""data:image/png;base64,(?<base64image>.+)\"")?>(?<title>.*)</A>$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        public static BookmarkFolder Parse(string contents)
        {
            int currentIndex = 0, currentDepth = 0;
            return new BookmarkFolder(DateTime.Now, "Bookmarks", DateTime.Now, ParseFolderContents(contents.Replace("<p>", ""), ref currentIndex, ref currentDepth));
        }

        private static List<BookmarkBase> ParseFolderContents(string contents, ref int currentIndex, ref int currentDepth)
        {
            var list = new List<BookmarkBase>();

            currentIndex = contents.IndexOf(folderContentStartTag, currentIndex);
            if (currentIndex == -1)
                throw new ArgumentException("Folder content start tag not found");
            currentIndex += folderContentStartTag.Length;

            while (true)
            {
                int bookmarkBaseIndex = contents.IndexOf(bookmarkBase, currentIndex);
                int folderContentEndIndex = contents.IndexOf(folderContentEndTag, currentIndex);

                if (folderContentEndIndex == -1)
                    throw new ArgumentException("Folder content end tag not found");

                if (bookmarkBaseIndex == -1 || folderContentEndIndex < bookmarkBaseIndex)
                {
                    currentIndex = folderContentEndIndex + folderContentEndTag.Length;
                    break;
                }

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

                    currentIndex = folderDescriptionEndIndex;
                    currentDepth++;
                    list.Add(new BookmarkFolder(addDate, title, lastModified, ParseFolderContents(contents, ref currentIndex, ref currentDepth)));
                    currentDepth--;
                }
                else if (bookmarkStartIndex == currentIndex)
                {
                    int bookmarkEndIndex = contents.IndexOf(bookmarkEndTag, bookmarkStartIndex + bookmarkStartTag.Length);
                    if (bookmarkEndIndex == -1)
                        throw new ArgumentException("Bookmark end tag not found");
                    bookmarkEndIndex += bookmarkEndTag.Length;

                    string bookmarkRaw = contents.Substring(bookmarkStartIndex, bookmarkEndIndex - bookmarkStartIndex);
                    list.Add(ParseBookmark(bookmarkRaw, true));

                    currentIndex = bookmarkEndIndex;
                }
                else throw new ArgumentException($"Unexpected tag after {bookmarkBase}");
            }

            return list;
        }

        private static void ParseFolderDescription(string contents, out DateTime addDate, out DateTime lastModified, out string title)
        {
            var match = folderDescriptionRegex.Match(contents);

            if (!match.Success)
                throw new ArgumentException("Invalid folder description");

            addDate = ParseDateTime(match.Groups["addDate"].Value);
            lastModified = ParseDateTime(match.Groups["lastModified"].Value);
            title = match.Groups["title"].Value;
        }

        private static Bookmark ParseBookmark(string contents, bool useRegex) // regex is slighty slower
        {
            if (useRegex)
            {
                var match = bookmarkRegex.Match(contents);

                if (!match.Success)
                    throw new ArgumentException("Invalid bookmark format");

                var addDate = ParseDateTime(match.Groups["addDate"].Value);
                var title = match.Groups["title"].Value;
                var link = match.Groups["link"].Value;

                return new Bookmark(addDate, title, link);
            }
            else
            {
                const int firstMark = 9;
                const int thirdMarkDifference = 12;
                const string iconPrefix = "\" ICON=\"data:image/png;base64,";

                int secondMark = contents.IndexOf('"', firstMark);
                var link = contents.Substring(firstMark, secondMark - firstMark);

                int thirdMark = secondMark + thirdMarkDifference;
                int fourthMark = contents.IndexOf('"', thirdMark);
                var rawAddDate = contents.Substring(thirdMark, fourthMark - thirdMark);

                int titleStart;
                if (contents.Substring(fourthMark, iconPrefix.Length) == iconPrefix)
                {
                    int iconStart = fourthMark + iconPrefix.Length;
                    int sixthMark = contents.IndexOf('"', iconStart);
                    var rawIcon = contents.Substring(iconStart, sixthMark - iconStart);
                    titleStart = sixthMark + 2;
                }
                else
                {
                    titleStart = fourthMark + 2;
                }

                int titleEnd = contents.IndexOf('<', titleStart);
                var title = contents.Substring(titleStart, titleEnd - titleStart);

                return new Bookmark(ParseDateTime(rawAddDate), title, link);
            }
        }

        private static DateTime ParseDateTime(string unixTimestamp) => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Int64.Parse(unixTimestamp)); // not sure if UTC though
    }
}
