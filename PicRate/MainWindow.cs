using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            var sw = new Stopwatch();
            sw.Start();
            var bookmarks = BookmarkParser.Parse(@"F:\foxneSs\backups\bookmarks\1000.html");
            sw.Stop();
            Debug.WriteLine(sw.Elapsed.TotalSeconds);

            //File.WriteAllBytes(@"C:\Users\foxneSs\Desktop\asd.txt", bookmarks.Serialize());
            //var bookmarks2 = BookmarkFolder.Deserialize(File.ReadAllBytes(@"C:\Users\foxneSs\Desktop\asd.txt"));
            //Debug.WriteLine(((Bookmark)((BookmarkFolder)bookmarks2[0])[500]).Link);
        }
    }
}
