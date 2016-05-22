using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;

namespace PicRate
{
    public partial class MainWindow : Form
    {
        private List<Tuple<Bookmark, RateableImage>> images = new List<Tuple<Bookmark, RateableImage>>();
        private Imgur imgur = new Imgur();
        private BookmarkFolderCache bookmarkCache = new BookmarkFolderCache(@"C:\Users\foxneSs\Desktop\Bookmark.cache");
        private List<Bookmark> imgurBookmarks;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            BookmarkFolder allBookmarks;
            var bookmarksRaw = File.ReadAllText(@"C:\Users\foxneSs\Desktop\large");
            if (bookmarkCache.ContainsKey(bookmarksRaw))
                allBookmarks = bookmarkCache[bookmarksRaw];
            else
            {
                bookmarkCache.Add(bookmarksRaw, JSONBookmarkParser.Parse(bookmarksRaw));
                allBookmarks = bookmarkCache[bookmarksRaw];
            }

            var nsfwFolder = (List<Bookmark>)allBookmarks.Find<BookmarkFolder>(bookmarkFolder => bookmarkFolder.Title == "nsfw")[0];
            imgurBookmarks = nsfwFolder.Where(bookmark => bookmark.Link.Contains("imgur.com")).ToList();

            ShowImage(0);
            imageList.Items.AddRange(imgurBookmarks.Select(bookmark => bookmark.Title).ToArray());

            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
        }

        private void ShowImage(int index)
        {
            var image = imgur.GetImage(imgurBookmarks[index].Link);
            if (image == null)
                return;

            images.Add(Tuple.Create(imgurBookmarks[index], new RateableImage(image)));
            imageBox.Image = images[index].Item2.Image;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            imgur.ImageCache.Save();
            imgur.ImageUrlCache.Save();
            bookmarkCache.Save();
        }
    }
}
