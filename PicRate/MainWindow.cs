using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PicRate
{
    public partial class MainWindow : Form
    {
        private List<Tuple<Bookmark, RateableImage>> images;
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
            var bookmarksRaw = File.ReadAllText(@"C:\Users\foxneSs\Desktop\large");
            if (!bookmarkCache.ContainsKey(bookmarksRaw))
                bookmarkCache.Add(bookmarksRaw, JSONBookmarkParser.Parse(bookmarksRaw));

            var allBookmarks = bookmarkCache[bookmarksRaw];

            var imageFolder = (List<Bookmark>)allBookmarks.Find<BookmarkFolder>(bookmarkFolder => bookmarkFolder.Title == "nsfw")[0];
            imgurBookmarks = imageFolder.Where(bookmark => bookmark.Link.Contains("imgur.com")).ToList();
            images = ListHelper.RepeatedDefault<Tuple<Bookmark, RateableImage>>(imgurBookmarks.Count);
            imageList.Items.AddRange(imgurBookmarks.Select(bookmark => bookmark.Title).ToArray());
            
            //imageList.SelectedIndex = 0;
        }

        private void ShowImage(int index)
        {
            if (images[index] == null)
            {
                Image image;
                try
                {
                    image = imgur.GetImage(imgurBookmarks[index].Link);
                }
                catch
                {
                    image = null;
                }
                images[index] = Tuple.Create(imgurBookmarks[index], new RateableImage(image));
            }

            imageBox.Image = images[index].Item2.Image;
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Hide();
            //imgur.ImageCache.Save();
            imgur.ImageUrlCache.Save();
            bookmarkCache.Save();
        }

        private void imageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowImage(imageList.SelectedIndex);
        }

        private void imageBox_DoubleClick(object sender, EventArgs e)
        {
            Process.Start(images[imageList.SelectedIndex].Item1.Link);
            BringToFront();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            imageList.SelectedIndex--;
        }

        private void forwardButton_Click(object sender, EventArgs e)
        {
            imageList.SelectedIndex++;
        }
    }
}
