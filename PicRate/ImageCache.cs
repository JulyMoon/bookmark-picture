using System.Drawing;

namespace PicRate
{
    class ImageCache : CacheBase<string, Image>
    {
        public ImageCache(string filePath) : base(filePath) { }
    }
}
