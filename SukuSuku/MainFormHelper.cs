using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
        /// <summary>
        /// 変数宣言
        /// </summary>
        string dirName = null;
        string orgText = ""; // 編集を始めた時点でのテキストボックスの内容
        Dictionary<string, Bitmap> templateBMPs = new Dictionary<string, Bitmap>();
        Dictionary<string, IplImage> templates = new Dictionary<string, IplImage>();
        int templateCnt = 0;
        Font defaultFontEditor = new Font("ＭＳ ゴシック", 12, System.Drawing.FontStyle.Regular); // デフォルトのエディタ用のフォント
        Font fontEditor = new Font("ＭＳ ゴシック", 12, System.Drawing.FontStyle.Regular); // 現在のエディタのフォント

        /// <summary>
        /// 上書き可能なビットマップデータを返す
        /// 普通にnew Bitmap(path)で得られたデータを使うと上書き保存できなくなる
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        Bitmap GetBitmapFromFile(string path)
        {
            using (var tmp = new Bitmap(path))
            {
                var bmp = new Bitmap(tmp.Width, tmp.Height);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(tmp, 0, 0, bmp.Width, bmp.Height);
                }
                return bmp;
            }
        }

        string GetImageName(int index) { return "image" + index; }

        /// <summary>
        /// 適当な画像名を返す
        /// </summary>
        /// <returns></returns>
        string GetImageName()
        {
            int i = 0;
            while (templates.Keys.Contains(GetImageName(i))) i++;
            return GetImageName(i);
        }

        /// <summary>
        /// サムネイル画像を作成
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        Image createThumbnail(Image image)
        {
            var w = thumbNailList.ImageSize.Width;
            var h = thumbNailList.ImageSize.Height;
            var canvas = new Bitmap(w, h);

            var g = Graphics.FromImage(canvas);
//            g.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, w, h);

            var fw = (float)w / (float)image.Width;
            var fh = (float)h / (float)image.Height;

            var scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            g.DrawImage(image, (w - fw) / 2, (h - fh) / 2, fw, fh);
            g.Dispose();

            return canvas;
        }

        void AddScreenshot(string imageKey, Bitmap bmp)
        {
            var index = thumbNailList.Images.Count;
            thumbNailList.Images.Add(imageKey, createThumbnail(bmp));
            if (thumbNailView.Items.ContainsKey(imageKey))
            {
                thumbNailView.Items[imageKey].ImageIndex = index;
            }
            else
            {
                thumbNailView.Items.Add(imageKey, imageKey, index);
                templateCnt++;
            }
            templates[imageKey] = BitmapConverter.ToIplImage(bmp);
            templateBMPs[imageKey] = bmp;
        }

        Bitmap DeleteScreenshot(string imageKey)
        {
            if (thumbNailView.Items.ContainsKey(imageKey) == false) return null;
            var x = thumbNailView.Items[0];
            var bmp = templateBMPs[imageKey];
            thumbNailView.Items.RemoveByKey(imageKey);
            thumbNailList.Images.RemoveByKey(imageKey);
            templates.Remove(imageKey);
            templateBMPs.Remove(imageKey);
            templateCnt--;
            return bmp;
        }

        void ClearScreenshots()
        {
            thumbNailView.Clear();
            thumbNailList.Images.Clear();
            templates.Clear();
            templateBMPs.Clear();
            templateCnt = 0;
        }

        /// <summary>
        /// エディタ画面に画像表示するしないの設定を反映させる
        /// </summary>
        void setTextBoxImages()
        {
            // 別スレッドから呼ばれることもあるのでInvokeにしておく
            Invoke((Action)(() =>
            {
                if (showScreenshotCheckBox.Checked)
                {
                    textBox.Image = templateBMPs;
                }
                else
                {
                    textBox.Image = new Dictionary<string, Bitmap>();
                }
                textBox.Refresh();
            }));
        }

        /// <summary>
        /// エディタ画面の画像を消す
        /// </summary>
        void clearTextBoxImages()
        {
            // 別スレッドから呼ばれることもあるのでInvokeにしておく
            Invoke((Action)(() =>
            {
                textBox.Image = new Dictionary<string, Bitmap>();
                textBox.Refresh();
            }));
        }
        //----------------------------------------------------------------------
        // ファイル
        //----------------------------------------------------------------------

        /// <summary>
        /// メインウインドウのタイトルを設定
        /// </summary>
        void SetTitle()
        {
            Text = "すくすく " + (dirName != null ? dirName : "");
        }

        /// <summary>
        /// 保存先のディレクトリ名を取得
        /// </summary>
        /// <returns>保存先のディレクトリ名</returns>
        string GetSaveDirName()
        {
            saveFileDialog.Filter = "ディレクトリ|*";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK) return saveFileDialog.FileName;
            return null;
        }

        /// <summary>
        /// 開きたいファイル群があるディレクトリ名を取得
        /// </summary>
        /// <returns>開きたいファイル群のあるディレクトリ名</returns>
        string GetOpenDirName()
        {
            openFileDialog.Filter = "ディレクトリ|*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK) return Path.GetDirectoryName(openFileDialog.FileName);
            return null;
        }

        /// <summary>
        /// 現在のテキストおよび撮影した画像を保存
        /// </summary>
        /// <param name="useDirName">dirNameをnullでないときに限り保存先のディレクトリとする</param>
        /// <returns>保存に成功したか</returns>
        bool Save(bool useDirName = true)
        {
            try
            {
                // 保存先を決める
                if (useDirName) dirName = dirName ?? GetSaveDirName();
                else dirName = GetSaveDirName();

                if (dirName == null)
                {
                    toolStripStatusLabel.Text = "保存に失敗しました";
                    return false;
                }

                // ディレクトリを作成
                Directory.CreateDirectory(dirName);

                // テキストを保存
                var path = Path.Combine(dirName, Path.GetFileName(dirName) + ".rb");
                var sw = new System.IO.StreamWriter(path);
                sw.Write(textBox.Text);
                sw.Close();
                orgText = textBox.Text;

                // 画像をpng形式で保存
                foreach (var pair in templates)
                {
                    path = Path.Combine(dirName, pair.Key + ".png");
                    File.Delete(path);
                    BitmapConverter.ToBitmap(pair.Value).Save(path, ImageFormat.Png);
                }
                templateCnt = templates.Count;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                toolStripStatusLabel.Text = "保存に失敗しました";
                return false;
            }

            toolStripStatusLabel.Text = "保存しました";
            return true;
        }

        /// <summary>
        /// テキスト・画像が変わったか判断し、変わったなら保存する
        /// </summary>
        /// <returns></returns>
        bool CheckSave()
        {
            if (orgText != textBox.Text || templates.Count != templateCnt)
            {
                switch (MessageBox.Show(
                    this,
                    "テキストが変更されています。保存しますか？",
                    "テキストの保存",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button3))
                {
                    case DialogResult.Yes:
                        return Save();
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 新規作成
        /// </summary>
        /// <returns></returns>
        bool NewFile()
        {
            // テキストボックス・サムネイルの内容をクリア
            dirName = null;
            textBox.Text = orgText = "";
            ClearScreenshots();
            return true;
        }

        /// <summary>
        /// 既存のファイルを開く
        /// </summary>
        /// <returns></returns>
        bool OpenFile()
        {
            // ディレクトリを取得
            dirName = GetOpenDirName() ?? dirName;
            if (dirName == null) return false;

            // テキストを開く
            var path = Path.Combine(dirName, Path.GetFileName(dirName) + ".rb");
            if (File.Exists(path))
            {
                var sr = new StreamReader(path);
                textBox.Text = orgText = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                textBox.Text = orgText = "";
            }

            // 画像を開く
            ClearScreenshots();
            foreach (var file in Directory.GetFiles(dirName, "*.png"))
            {
                var bmp = GetBitmapFromFile(file);
                var imageName = Path.GetFileNameWithoutExtension(file);
                AddScreenshot(imageName, bmp);
            }
            
            return true;
        }

        //----------------------------------------------------------------------
        // 実行
        //----------------------------------------------------------------------

        /// <summary>
        /// rectの領域内のスクリーンショットを撮ってbitmapデータを返す
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        private Bitmap GetScreenshotBmp(Rectangle rect)
        {
            var size = new Size(rect.Width, rect.Height);
            var bmp = new Bitmap(size.Width, size.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                clearTextBoxImages();   // エディタ画面の画像を一旦消す（エディタ画面の画像を認識してしまうのを防ぐため。ちょっとちらつくけど仕方ない）
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, size, CopyPixelOperation.SourceCopy);
                setTextBoxImages();
            }
            
            return bmp;
        }

        /// <summary>
        ///  rectの領域内のスクリーンショットを撮って保存
        /// </summary>
        /// <param name="rect"></param>
        public string takeScreenshot(Rectangle rect)
        {
            var bmp = GetScreenshotBmp(rect);
            var imageName = GetImageName();
            AddScreenshot(imageName, bmp);
            return imageName;
        }

        /// <summary>
        /// スクリーン画像とimageNameで指定した画像とのマッチング。Cv.MatchingTemplate()を利用
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public Rectangle findTemplate(string imageName, double threshold = -1, bool showNotFoundDialog = true)
        {
            if (templates.Keys.Contains(imageName) == false)
            {
                MessageBox.Show("Error! Invalid image name");
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
            var tmpl = templates[imageName];

            // 画像マッチング
            double min_val = 0, max_val = 0;
            CvPoint min_loc = CvPoint.Empty, max_loc = CvPoint.Empty;

            // 異なる解像度の画像を用意し小さい方から順に試していく
            // 期待値的に高速になるはず
            var invratio = 1.0;
            foreach (var ratio in new[] { 0.25, 0.5/*, 0.75*/, 1.0 })
            {
#if DEBUG
                Console.Write("[ratio = " + ratio + "] "); 
                var stopwatch2 = new System.Diagnostics.Stopwatch();
                stopwatch2.Start();
#endif
                // 縮小した画像を用意する
                var small_target = new IplImage((int)(target.Size.Width * ratio), (int)(target.Size.Height * ratio), target.Depth, target.NChannels);
                var small_tmpl = new IplImage((int)(tmpl.Size.Width * ratio), (int)(tmpl.Size.Height * ratio), tmpl.Depth, tmpl.NChannels);

                // 小さすぎたらマッチングは行わない
                if (small_tmpl.Width <= 0 || small_tmpl.Height <= 0) continue;

                target.Resize(small_target);
                tmpl.Resize(small_tmpl);

                // マッチング
                var dstSize = new CvSize(small_target.Width - small_tmpl.Width + 1, small_target.Height - small_tmpl.Height + 1);
                using (var dst = Cv.CreateImage(dstSize, BitDepth.F32, 1))
                {
                    Cv.MatchTemplate(small_target, small_tmpl, dst, MatchTemplateMethod.CCoeffNormed);
                    Cv.MinMaxLoc(dst, out min_val, out max_val, out min_loc, out max_loc, null);
                }
#if DEBUG
                stopwatch2.Stop(); 
                Console.WriteLine("max_val = " + max_val + "(" + stopwatch2.Elapsed.TotalSeconds + " s)");
#endif
                invratio = 1 / ratio;

                // 閾値以上のマッチ率が得られたら打ち切る
                if (threshold <= max_val) break;
            }

            // 時間計測終了
            stopwatch.Stop();
#if DEBUG
            Console.WriteLine("Ellapsed Time = " + stopwatch.Elapsed.TotalSeconds + " s");
            //Console.WriteLine("min_val = " + min_val);
            //Console.WriteLine("min_loc = " + min_loc);
            Console.WriteLine("max_val = " + max_val);
            //Console.WriteLine("max_loc = " + max_loc);
            Console.WriteLine("");
#endif

            // max_valがthresholdを超えなければマッチしなかったと判断する
            if (threshold > max_val)
            {
                if (showNotFoundDialog) MessageBox.Show("画像 " + imageName + " はスクリーン内で見つかりませんでした。");
                return Rectangle.Empty;
            }

            return new Rectangle((int)(max_loc.X * invratio), (int)(max_loc.Y * invratio), tmpl.Size.Width, tmpl.Size.Height);
        }

        //----------------------------------------------------------------------
        // 表示
        //----------------------------------------------------------------------

        /// <summary>
        /// エディタのフォントを変更する
        /// </summary>
        /// <param name="font"></param>
        private void SetFontEditor(Font font)
        {
            textBox.Font = fontEditor = font;
        }
    }
}
