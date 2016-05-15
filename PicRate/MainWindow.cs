using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
            var bookmarks = BookmarkParser.Parse(@"F:\foxneSs\backups\bookmarks\bookmarks_5_9_16.html");
            sw.Stop();
            Debug.WriteLine(sw.Elapsed.TotalSeconds);
        }
    }
}
