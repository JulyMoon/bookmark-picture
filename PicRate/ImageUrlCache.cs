namespace PicRate
{
    class ImageUrlCache : CacheBase<string, string>
    {
        public ImageUrlCache(string filePath) : base(filePath) { }
    }
}
