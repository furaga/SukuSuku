using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using OpenCvSharp;

namespace SukuSuku
{
    /// <summary>
    /// メインフォームの補助関数群
    /// </summary>
    public partial class MainForm : Form
    {
        //----------------------------------------------------------------------
        // 変数宣言
        //----------------------------------------------------------------------

        string dirName = null;  // 編集しているスクリプトのディレクトリ
        string orgText = "";    // 編集を始めた時点でのテキストボックスの内容
        int orgTemplateCnt = 0;    // 編集を始めた時点でのテンプレート画像の数

        // 画像名とテンプレート画像の辞書
        // マッチングのたびにBitmap => IplImageと変換するのはコストがかかりそうなのであらかじめ用意しておく
        Dictionary<string, Bitmap> templateBMPs = new Dictionary<string, Bitmap>();     // Bitmap型
        Dictionary<string, IplImage> templates = new Dictionary<string, IplImage>();    // IplImage型。

        Font defaultFontEditor = new Font("ＭＳ ゴシック", 12, System.Drawing.FontStyle.Regular); // デフォルトのエディタ用のフォント
        Font fontEditor = new Font("ＭＳ ゴシック", 12, System.Drawing.FontStyle.Regular);        // 現在のエディタのフォント


        //----------------------------------------------------------------------
        //
        // 補助関数群
        //
        //----------------------------------------------------------------------

        /// <summary>
        /// 整数値から適当な画像名を返す
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string GetImageName(int n) { return "image" + n; }

        /// <summary>
        /// 適当な画像名を返す
        /// </summary>
        /// <returns></returns>
        public string GetImageName()
        {
            // "imageX"という名前でまだ使われていないXを見つける
            int i = 0;
            while (templates.Keys.Contains(GetImageName(i))) i++;
            return GetImageName(i);
        }

        /// <summary>
        /// 上書き可能なビットマップデータを返す
        /// 普通にnew Bitmap(path)で得られたデータを使うと上書き保存できなくなる
        /// </summary>
        /// <param name="path">画像ファイルへのパス</param>
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

        /// <summary>
        /// サムネイル画像を作成
        /// </summary>
        /// <param name="image">元画像</param>
        /// <returns></returns>
        Image CreateThumbnail(Image image)
        {
            var w = thumbNailList.ImageSize.Width;
            var h = thumbNailList.ImageSize.Height;
            var fw = (float)w / (float)image.Width;
            var fh = (float)h / (float)image.Height;
            var scale = Math.Min(fw, fh);
            fw = image.Width * scale;
            fh = image.Height * scale;

            var canvas = new Bitmap(w, h);

            using (var g = Graphics.FromImage(canvas))
            {
                g.DrawImage(image, (w - fw) / 2, (h - fh) / 2, fw, fh);
            }

            return canvas;
        }

        //----------------------------------------------------------------------
        // スクリーンショット画像の追加・削除・クリア
        //----------------------------------------------------------------------

        /// <summary>
        /// スクリーンショット画像をバッファに追加
        /// </summary>
        /// <param name="imageKey">画像名</param>
        /// <param name="bmp">画像データ</param>
        void AddScreenshot(string imageKey, Bitmap bmp)
        {
            var index = thumbNailList.Images.Count;
            thumbNailList.Images.Add(imageKey, CreateThumbnail(bmp));
            if (thumbNailView.Items.ContainsKey(imageKey))
            {
                thumbNailView.Items[imageKey].ImageIndex = index;
            }
            else
            {
                thumbNailView.Items.Add(imageKey, imageKey, index);
            }
            templates[imageKey] = BitmapConverter.ToIplImage(bmp);
            templateBMPs[imageKey] = bmp;
        }

        /// <summary>
        /// スクリーンショット画像をバッファから消す
        /// </summary>
        /// <param name="imageKey">消したい画像の名前</param>
        /// <returns>消した画像のビットマップデータ</returns>
        Bitmap DeleteScreenshot(string imageKey)
        {
            if (thumbNailView.Items.ContainsKey(imageKey) == false) return null;
            var bmp = templateBMPs[imageKey];
            thumbNailList.Images[thumbNailView.Items.IndexOfKey(imageKey)].Dispose();
//            thumbNailList.Images.RemoveAt(thumbNailView.Items.IndexOfKey(imageKey)); // これをやるとインデックスがずれて変なことになる
            thumbNailView.Items.RemoveByKey(imageKey);
            templates.Remove(imageKey);
            templateBMPs.Remove(imageKey);
            return bmp;
        }

        /// <summary>
        /// バッファからスクリーンショット画像をすべて消す
        /// </summary>
        void ClearScreenshots()
        {
            thumbNailView.Clear();
            foreach (Image x in thumbNailList.Images) x.Dispose();
            thumbNailList.Images.Clear();
            foreach (var img in templates.Values) img.Dispose();
            templates.Clear();
            foreach (var img in templateBMPs.Values) img.Dispose();
            templateBMPs.Clear();
        }

        //----------------------------------------------------------------------
        // エディタ画面への画像表示
        //----------------------------------------------------------------------
        
        /// <summary>
        /// エディタ画面に画像表示/非表示の設定を反映させる
        /// </summary>
        void SetTextBoxImages()
        {
            // 別スレッドから呼ばれることもあるのでInvokeにしておく
            Invoke((Action)(() =>
            {
                textBox.Images = showScreenshotCheckBox.Checked ? templateBMPs : null;
                textBox.Refresh();
            }));
        }

        /// <summary>
        /// エディタ画面の画像を消す
        /// </summary>
        void ClearTextBoxImages()
        {
            // 別スレッドから呼ばれることもあるのでInvokeにしておく
            Invoke((Action)(() =>
            {
                textBox.Images = null;
                textBox.Refresh();
            }));
        }


        //----------------------------------------------------------------------
        // ファイル
        //----------------------------------------------------------------------

        /// <summary>
        /// メインウインドウのタイトルを設定
        /// </summary>
        void SetTitle() { Text = "すくすく " + (dirName != null ? dirName : ""); }

        /// <summary>
        /// 保存先のディレクトリ名を取得
        /// </summary>
        /// <returns>保存先のディレクトリ名</returns>
        string GetSaveDirName()
        {
            saveFileDialog.Title = "保存先のディレクトリ名を入力してください";
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
            openFileDialog.Title = "スクリプトファイルを選択してください";
            openFileDialog.Filter = "Rubyファイル(*.rb)|*.rb";
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
                orgTemplateCnt = templates.Count;
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
            if (orgText != textBox.Text || templates.Count != orgTemplateCnt)
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
                        Save();
                        return true;
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
            orgTemplateCnt = 0;
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
            orgTemplateCnt = 0;
            foreach (var file in Directory.GetFiles(dirName, "*.png"))
            {
                var bmp = GetBitmapFromFile(file);
                var imageName = Path.GetFileNameWithoutExtension(file);
                AddScreenshot(imageName, bmp);
                orgTemplateCnt++;
            }
            
            return true;
        }

        //----------------------------------------------------------------------
        // 実行
        //----------------------------------------------------------------------


        void SetPlayButtons(bool startPlaying)
        {
            // 実行中に実行ボタンが押されたり、スクリーンショットリストの内容が変わらないようにする
            実行RToolStripMenuItem1.Enabled =
            スローモーションで実行RToolStripMenuItem.Enabled =
            runButton.Enabled =
            runSlowlyButton.Enabled =
            screenshotButton.Enabled =
            スクリーンショットを撮るSToolStripMenuItem.Enabled =
            autoChapCheckBox.Checked =
            autoChapCheckBox.Enabled =
            thumbNailContextMenuStrip.Enabled =
            !startPlaying;

            停止SToolStripMenuItem.Enabled =
            stopButton.Enabled = startPlaying;
        }

        /// <summary>
        /// スクリプトを実行する
        /// </summary>
        void Run(string sourceCode)
        {
            // 実行中のスレッドは殺す
            if (thread != null && thread.IsAlive) thread.Abort();

            // スレッドを新しく作る
            thread = new System.Threading.Thread(() =>
            {
                try
                {
                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(true);
                        toolStripStatusLabel.Text = "実行開始";
                    }));

                    engine.Execute(sourceCode, scope);

                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(false);
                        toolStripStatusLabel.Text = "正常終了";
                    }));
                }
                catch (System.Threading.ThreadAbortException)
                {
                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(false);
                        toolStripStatusLabel.Text = "異常終了";
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(false);
                        toolStripStatusLabel.Text = "異常終了";
                    }));
                }
                finally
                {
                    MessageBox.Show("プロセスは終了しました");
                }
            });

            // スレッド開始
            thread.Start();
        }

        /// <summary>
        /// スクリプトを実行する（スクリプトファイル指定版）
        /// </summary>
        /// <param name="scriptPath">実行するスクリプトのパス</param>
        void RunByScriptPath(string scriptPath)
        {
            // Run()と大体同じだけど、スレッドの処理がところどころ違っていて統合しにくい

            // 実行中のスレッドは殺す
            if (thread != null && thread.IsAlive) thread.Abort();

            // スレッドを新しく作る
            thread = new System.Threading.Thread(() =>
            {
                var tmp = templates;
                try
                {
                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(true);
                        toolStripStatusLabel.Text = "実行開始";
                    }));

                    templates = new Dictionary<string, IplImage>();
                    foreach (var fileName in Directory.GetFiles(Path.GetDirectoryName(scriptPath), "*.png"))
                    {
                        var bmp = GetBitmapFromFile(fileName);
                        var imageName = Path.GetFileNameWithoutExtension(fileName);
                        templates.Add(imageName, BitmapConverter.ToIplImage(bmp));
                    }
                    using (var sr = new StreamReader(scriptPath))
                    {
                        engine.Execute(sr.ReadToEnd(), scope);
                    }

                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(false);
                        toolStripStatusLabel.Text = "正常終了";
                    }));
                }
                catch (System.Threading.ThreadAbortException)
                {
                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(false);
                        toolStripStatusLabel.Text = "異常終了";
                    }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    Invoke((Action)(() =>
                    {
                        SetPlayButtons(false);
                        toolStripStatusLabel.Text = "異常終了";
                    }));
                }
                finally
                {
                    foreach (var img in templates.Values) img.Dispose();
                    templates = tmp;
                    MessageBox.Show(scriptPath + "は終了しました");
                }
            });

            // スレッド開始
            thread.Start();
        }

        /// <summary>
        /// rectの領域内のスクリーンショットを撮ってbitmapデータを返す
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Bitmap TakeScreenshot(Rectangle rect)
        {
            var size = new Size(rect.Width, rect.Height);
            var bmp = new Bitmap(size.Width, size.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                ClearTextBoxImages();   // エディタ画面の画像を一旦消す（エディタ画面の画像を認識してしまうのを防ぐため。ちょっとちらつくけど仕方ない）
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, size, CopyPixelOperation.SourceCopy);
                SetTextBoxImages();
            }
            
            return bmp;
        }

        /// <summary>
        ///  rectの領域内のスクリーンショットを撮って保存
        /// </summary>
        /// <param name="rect"></param>
        public string TakeAndAddScreenshot(Rectangle rect, string defaultImageName = null)
        {
            var bmp = TakeScreenshot(rect);
            var imageName = defaultImageName ?? GetImageName();
            Invoke((Action)(() =>
            {
                AddScreenshot(imageName, bmp);
                textBox.Refresh();
            }));
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

            // thresholdが不当な値だったらthresholdUpDownの数値を使う
            if (threshold < 0 || 100 < threshold) threshold = (double)thresholdUpDown.Value;

            // 0 ~ 1.0にスケールを合わせる
            threshold *= 0.01;

            // 時間計測開始
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // テンプレート画像を用意
            var tmpl = templates[imageName];

            // 画像マッチング
            double min_val = 0, max_val = 0;
            CvPoint min_loc = CvPoint.Empty, max_loc = CvPoint.Empty;

            var invratio = 1.0;

            // 異なる解像度の画像を用意し小さい方から順に試していく
            // 期待値的に高速になるはず
            foreach (var ratio in new[] { 0.5/*, 0.5, 0.75*/, 1.0 })
            {
#if DEBUG
                Console.Write("[ratio = " + ratio + "] "); 
                var stopwatch2 = new System.Diagnostics.Stopwatch();
                stopwatch2.Start();
#endif
                // ビットマップ・IplImageは明示的に開放しないとメモリリークする
                using (var bmp = TakeScreenshot(Screen.PrimaryScreen.Bounds))
                using (var target = BitmapConverter.ToIplImage(bmp))
                using (var small_target = new IplImage((int)(target.Size.Width * ratio), (int)(target.Size.Height * ratio), target.Depth, target.NChannels))
                using (var small_tmpl = new IplImage((int)(tmpl.Size.Width * ratio), (int)(tmpl.Size.Height * ratio), tmpl.Depth, tmpl.NChannels))
                {
                    // 小さすぎたらマッチングは行わない
                    if (small_tmpl.Width <= 0 || small_tmpl.Height <= 0) continue;

                    // 縮小した画像を用意
                    target.Resize(small_target);
                    tmpl.Resize(small_tmpl);

                    // 画像マッチング
                    var dstSize = new CvSize(small_target.Width - small_tmpl.Width + 1, small_target.Height - small_tmpl.Height + 1);
                    using (var dst = Cv.CreateImage(dstSize, BitDepth.F32, 1))
                    {
                        Cv.MatchTemplate(small_target, small_tmpl, dst, MatchTemplateMethod.CCoeffNormed);
                        Cv.MinMaxLoc(dst, out min_val, out max_val, out min_loc, out max_loc, null);
                    }
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

        //----------------------------------------------------------------------
        // ホットキー（ショートカットキー）の登録
        //----------------------------------------------------------------------

        /// <summary>
        /// ホットキーを登録する
        /// </summary>
        /// <param name="modifiers">修飾キー</param>
        /// <param name="key">キーコード</param>
        /// <param name="text">ホットキーに対する注釈</param>
        /// <param name="action">ホットキーが押されたときの動作</param>
        void AddHotKeyAction(uint modifiers, Keys key, string text, Action action)
        {
            var lparam = new IntPtr(modifiers | ((uint)key * 0x10000));
            RegisterHotKey(this.Handle, (Int32)lparam, modifiers, Convert.ToUInt32(key));
            hotKeyActions[lparam] = new Tuple<string, Action>(text, action);
        }

        /// <summary>
        /// ホットキーを登録する（実行するスクリプトを指定）
        /// </summary>
        /// <param name="modifiers">修飾キー</param>
        /// <param name="key">キーコード</param>
        /// <param name="scriptPath">実行するスクリプトへのパス</param>
        public void AddHotKeyActionByScript(uint modifiers, Keys key, string scriptPath)
        {
            AddHotKeyAction(modifiers, key, scriptPath, () => { ui.slowPlayFlag = true; RunByScriptPath(scriptPath); });
        }

        /// <summary>
        /// 設定ファイル(configPath)の内容を読みこんでホットキーを適宜登録する
        /// </summary>
        void LoadConfigData()
        {
            foreach (var line in System.IO.File.ReadLines(ShortcutForm.configPath).Where(s => s.Trim().Length >= 1 && s.Trim()[0] != '#'))
            {
                var tokens = line.Split(new[] { ',' }).Select(s => s.Trim()).ToArray();
                if (tokens.Length != 2) continue;
                uint id;
                if (uint.TryParse(tokens[0], out id) == false) continue;   // IDをパース
                if (tokens[1].EndsWith(".rb") == false || System.IO.File.Exists(tokens[1]) == false) continue; // ファイル名が不正でないか
                AddHotKeyActionByScript(id & 0xffff, (Keys)(id / 0x10000), tokens[1]);
            }
        }

    }
}
