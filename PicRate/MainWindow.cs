using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PicRate
{
    public partial class MainWindow : Form
    {
        private List<Tuple<Bookmark, RateableImage>> images = new List<Tuple<Bookmark, RateableImage>>();
        private Imgur imgur = new Imgur();
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
            

            var allBookmarks = JSONBookmarkParser.Parse(File.ReadAllText(@"C:\Users\foxneSs\Desktop\large"));
            var nsfwFolder = (List<Bookmark>)allBookmarks.Find<BookmarkFolder>(bookmarkFolder => bookmarkFolder.Title == "nsfw")[0];
            imgurBookmarks = nsfwFolder.Where(bookmark => bookmark.Link.Contains("imgur.com")).ToList();

            sw.Start();
            foreach (var imgurBookmark in imgurBookmarks)
            {
                imageList.Items.Add(imgurBookmark.Title);
            }
            sw.Stop();
            ShowImage(0);

            
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
    }
}
