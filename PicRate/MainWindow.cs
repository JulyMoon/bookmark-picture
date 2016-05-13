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
            var bookmarks = new BookmarkCollection(@"F:\foxneSs\backups\bookmarks\5000.html");
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds / 1000d);
        }
    }
}
