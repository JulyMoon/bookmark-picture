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

            var allBookmarks = JSONBookmarkParser.Parse(File.ReadAllText(@"C:\Users\foxneSs\Desktop\large"));
            var nsfwFolder = (List<Bookmark>)allBookmarks.Find<BookmarkFolder>(bookmarkFolder => bookmarkFolder.Title == "nsfw")[0];
            var imgurBookmarks = nsfwFolder.Where(bookmark => bookmark.Link.Contains("imgur.com")).ToList();
            images.Add(Tuple.Create(imgurBookmarks[0], new RateableImage(imgur.GetImage(imgurBookmarks[0].Link))));
            ShowImage(0);

            sw.Stop();
            Debug.WriteLine(sw.Elapsed);
        }

        private void ShowImage(int index)
        {
            imageBox.Image = images[index].Item2.Image;
        }
    }
}
