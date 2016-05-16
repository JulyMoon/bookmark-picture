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
            var cache = new BookmarkFolderCache(cacheFile);
            var bookmarks = cache.Add(File.ReadAllBytes(@"F:\foxneSs\backups\bookmarks\10000.html"));
            cache.Save();
            sw.Stop();
            Debug.WriteLine(sw.Elapsed.TotalSeconds);
        }
    }
}
