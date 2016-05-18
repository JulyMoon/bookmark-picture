using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace PicRate
{
    public partial class MainWindow : Form
    {
        private const string cacheFile = @"C:\Users\foxneSs\Desktop\Cache.dat";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var bookmarks = JSONBookmarkParser.Parse(File.ReadAllText(@"C:\Users\foxneSs\Desktop\large"));
            sw.Stop();
            Debug.WriteLine(sw.Elapsed.TotalSeconds);

            sw.Reset();
            sw.Start();
            bookmarks = bookmarks.Flattened();
            int count = 0;
            for (int i = 0; i < bookmarks.Count; i++)
                for (int j = 0; j < bookmarks.Count; j++)
                {
                    if (i == j)
                        continue;

                    var a = (Bookmark)bookmarks[i];
                    var b = (Bookmark)bookmarks[j];

                    if (a.Link == b.Link && a.Title == b.Title)
                        count++;
                }
            Debug.WriteLine($"{count} matches found in total");
            sw.Stop();
            Debug.WriteLine(sw.Elapsed.TotalSeconds);
        }
    }
}
