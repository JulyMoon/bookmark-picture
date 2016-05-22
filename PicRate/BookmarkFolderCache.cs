namespace PicRate
{
    class BookmarkFolderCache : CacheBase<string, BookmarkFolder>
    {
        public BookmarkFolderCache(string filePath) : base(filePath) { }
    }
}
