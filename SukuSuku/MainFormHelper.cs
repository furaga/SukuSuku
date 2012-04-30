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
        List<IplImage> templates = new List<IplImage>();

        private Bitmap GetScreenshotBmp(Rectangle rect)
        {
            var size = new Size(rect.Width, rect.Height);
            var bmp = new Bitmap(size.Width, size.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, size, CopyPixelOperation.SourceCopy);
            }

            return bmp;
        }

        public void takeScreenshot(Rectangle rect)
        {
            var bmp = GetScreenshotBmp(rect);
            var index = thumbNailList.Images.Count;
            thumbNailList.Images.Add(bmp);
            thumbNailView.Items.Add("image" + index, index);
            templates.Add(BitmapConverter.ToIplImage(bmp));
        }

        public Point? findTemplate(int imageIndex)
        {
            if (imageIndex >= templates.Count)
            {
                MessageBox.Show("Error! Out of index");
                return null;
            }

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            var bmp = GetScreenshotBmp(Screen.PrimaryScreen.Bounds);
            var target = BitmapConverter.ToIplImage(bmp);

            /*
            using (var win = new CvWindow("win", BitmapConverter.ToIplImage(bmp)))
            {
                CvWindow.WaitKey();
            }
            */

            var tmpl = templates[imageIndex];

            double min_val, max_val;
            CvPoint min_loc, max_loc;
            var size = new CvSize(target.Width - tmpl.Width + 1, target.Height - tmpl.Height + 1);
            using (var dst = Cv.CreateImage(size, BitDepth.F32, 1))
            {
                Cv.MatchTemplate(target, tmpl, dst, MatchTemplateMethod.CCoeffNormed);
                Cv.MinMaxLoc(dst, out min_val, out max_val, out min_loc, out max_loc, null);
            }

            stopwatch.Stop();

            Console.WriteLine("Ellapsed Time = " + stopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("min_val = " + min_val);
            Console.WriteLine("min_loc = " + min_loc);
            Console.WriteLine("max_val = " + max_val);
            Console.WriteLine("max_loc = " + max_loc);

            var x = max_loc.X + tmpl.Size.Width / 2;
            var y = max_loc.Y + tmpl.Size.Height / 2;

            return new Point(x, y);
        }
    }
}
