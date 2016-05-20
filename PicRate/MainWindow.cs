using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PicRate
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            var bookmarks = JSONBookmarkParser.Parse(File.ReadAllText(@"C:\Users\foxneSs\Desktop\large"));
            
            var nsfwFolder = (List<Bookmark>)bookmarks.Find<BookmarkFolder>(bookmarkFolder => bookmarkFolder.Title == "nsfw")[0];
            var urls = nsfwFolder.Select(bookmark => bookmark.Link).Where(link => link.Contains("imgur.com")).ToList();
            var urlString = urls.Aggregate((a, b) => $"{a}\n{b}");
            //var imgur = new Imgur();
            //var asd = imgur.GetImages(urls);
        }
    }
}
