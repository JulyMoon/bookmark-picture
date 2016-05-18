using System;
using System.Collections;
using System.Collections.Generic;

namespace PicRate
{
    [Serializable]
    class BookmarkFolder : BookmarkBase, IReadOnlyList<BookmarkBase>
    {
        public readonly DateTime LastModified;
        private readonly List<BookmarkBase> collection;

        public int Count => collection.Count;

        public BookmarkBase this[int number] => collection[number];

        public BookmarkFolder(DateTime addDate, string title, DateTime lastModified, List<BookmarkBase> bookmarkBases) : base(addDate, title)
        {
            LastModified = lastModified;
            collection = bookmarkBases;
        }

        public BookmarkFolder Flattened()
        {
            var newCollection = new List<BookmarkBase>();
            foreach (var bookmarkBase in collection)
            {
                if (bookmarkBase is Bookmark)
                    newCollection.Add(bookmarkBase);
                else // if (bookmarkBase is BookmarkFolder)
                {
                    foreach (var bookmark in ((BookmarkFolder)bookmarkBase).Flattened().collection)
                        newCollection.Add(bookmark);
                }
            }

            return new BookmarkFolder(AddDate, Title, LastModified, newCollection);
        }

        public IEnumerator GetEnumerator() => collection.GetEnumerator();

        IEnumerator<BookmarkBase> IEnumerable<BookmarkBase>.GetEnumerator() => collection.GetEnumerator();
    }

    [Serializable]
    class Bookmark : BookmarkBase
    {
        public readonly string Link;
        // public Bitmap Icon; TODO

        public Bookmark(DateTime addDate, string title, string link) : base(addDate, title)
        {
            Link = link;
        }
    }

    [Serializable]
    abstract class BookmarkBase
    {
        public readonly DateTime AddDate;
        public readonly string Title;

        public BookmarkBase(DateTime addDate, string title)
        {
            AddDate = addDate;
            Title = title;
        }

        public override string ToString() => Title;
    }
}