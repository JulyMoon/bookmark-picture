using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;

namespace PicRate
{
    static class JSONBookmarkParser
    {
        public static BookmarkFolder Parse(string rawJson)
        {
            var root = (JObject)JObject.Parse(rawJson)["roots"];
            root.Remove("sync_transaction_version");
            return new BookmarkFolder(DateTime.Now, "Bookmarks", DateTime.Now, ParseFolderContents(new JArray(root.Children().Select(a => a.First).ToArray())));
        }

        private static List<BookmarkBase> ParseFolderContents(JArray folderContents)
        {
            var collection = new List<BookmarkBase>();

            foreach (var bookmarkBase in folderContents)
            {
                var title = bookmarkBase.Value<string>("name");
                var addDate = ParseDateTime(bookmarkBase.Value<long>("date_added"));

                switch (bookmarkBase.Value<string>("type"))
                {
                    case "folder":
                        var lastModified = ParseDateTime(bookmarkBase.Value<long>("date_modified"));
                        collection.Add(new BookmarkFolder(addDate, title, lastModified, ParseFolderContents((JArray)bookmarkBase["children"])));
                        break;
                    case "url":
                        var link = bookmarkBase.Value<string>("url");
                        collection.Add(new Bookmark(addDate, title, link));
                        break;
                    default: throw new ArgumentException($"Unexpected BookmarkBase type: {bookmarkBase.Value<string>("type")}");
                }
            }

            return collection;
        }

        private static DateTime ParseDateTime(long raw) => new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(raw / 1000d); // not sure if UTC
    }
}
