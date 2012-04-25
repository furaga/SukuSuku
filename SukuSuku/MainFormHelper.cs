using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenCvSharp;
using Sgry.Azuki.Highlighter;

namespace SukuSuku
{
    public partial class MainForm : Form
    {
        [DllImport("user32")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public void takeScreenshot(Rectangle rect)
        {
            var size = new Size(rect.Width, rect.Height);
            var index = thumbNailList.Images.Count;
            var bmp = new Bitmap(size.Width, size.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, size, CopyPixelOperation.SourceCopy);
            }
            thumbNailList.Images.Add(bmp);
            thumbNailView.Items.Add("image" + index, index);
        }
    }
}
