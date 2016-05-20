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

            var imgurRegex = new Regex(@"imgur", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            var nsfwFolder = (List<Bookmark>)bookmarks.Find<BookmarkFolder>(bookmarkFolder => bookmarkFolder.Title == "nsfw")[0];
            var urls = nsfwFolder.Select(bookmark => bookmark.Link).Where(link => imgurRegex.IsMatch(link)).ToList().Aggregate((a, b) => a + "\n" + b);
        }
    }
}
