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
using Sgry.Azuki.Windows;

namespace SukuSuku
{
    public partial class MainForm : Form
    {
        List<IplImage> templates = new List<IplImage>();

        // rectの領域内のスクリーンショットを撮ってbitmapデータを返す
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

        // rectの領域内のスクリーンショットを撮って保存
        public void takeScreenshot(Rectangle rect)
        {
            var bmp = GetScreenshotBmp(rect);
            var index = thumbNailList.Images.Count;
            thumbNailList.Images.Add(bmp);
            thumbNailView.Items.Add("image" + index, index);
            templates.Add(BitmapConverter.ToIplImage(bmp));
        }

        // スクリーン画像とimageNameで指定した画像とのマッチング
        public Rectangle findTemplate(string imageName, double threshold = -1)
        {
            var imageIndex = imageName.Last() - '0';

            if (imageIndex >= templates.Count)
            {
                MessageBox.Show("Error! Out of index");
                return Rectangle.Empty;
            }

            // thresholdが不当な値だったらthresholdUpDownに書かれている数値を使う
            if (threshold < 0 || 100 < threshold) threshold = (double)thresholdUpDown.Value;

            // 0 ~ 1.0にスケールを合わせる
            threshold *= 0.01;

            // 時間計測開始
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // 比較する2つの画像を用意
            var bmp = GetScreenshotBmp(Screen.PrimaryScreen.Bounds);
            var target = BitmapConverter.ToIplImage(bmp);
            var tmpl = templates[imageIndex];

            // 画像マッチング
            double min_val, max_val;
            CvPoint min_loc, max_loc;

            var size = new CvSize(target.Width - tmpl.Width + 1, target.Height - tmpl.Height + 1);
            using (var dst = Cv.CreateImage(size, BitDepth.F32, 1))
            {
                Cv.MatchTemplate(target, tmpl, dst, MatchTemplateMethod.CCoeffNormed);
                Cv.MinMaxLoc(dst, out min_val, out max_val, out min_loc, out max_loc, null);
            }

            // 時間計測終了
            stopwatch.Stop();

            Console.WriteLine("Ellapsed Time = " + stopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("min_val = " + min_val);
            Console.WriteLine("min_loc = " + min_loc);
            Console.WriteLine("max_val = " + max_val);
            Console.WriteLine("max_loc = " + max_loc);

            var x = max_loc.X + tmpl.Size.Width / 2;
            var y = max_loc.Y + tmpl.Size.Height / 2;

            // max_valがthresholdを超えなければマッチしなかったと判断する
            if (threshold > max_val)
            {
                MessageBox.Show("画像 " + imageName + " はスクリーン内で見つかりませんでした。");
                return Rectangle.Empty;
            }

            return new Rectangle(max_loc.X, max_loc.Y, tmpl.Size.Width, tmpl.Size.Height);
        }
    }
}
