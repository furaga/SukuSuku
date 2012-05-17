using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace SukuSuku
{
    /// <summary>
    /// GUI操作関数群を定義しているクラス
    // スクリプト側ではこの中の関数が呼ばれることでGUI操作を行う
    /// </summary>
    class UI
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010,
            WHEEL = 0x00000800,
            XDOWN = 0x00000080,
            XUP = 0x00000100
        }

        // メインフォームへの参照
        MainForm owner;

        public bool slowPlayFlag = false;

        // コンスタラクタ
        public UI(MainForm owner)
        {
            this.owner = owner;
        }

        // MainForm.findTemplate()で帰ってきた矩形の中心座標を返す(Rectangle.Emptyだったらnullを返す)
        Point? findTemplatePoint(string imageName, double threshold)
        {
            //var rect = owner.findTemplate(imageName, threshold);

            // 見つかるまで待つ
            var rect = wait(imageName, -1, threshold);
            if (rect == Rectangle.Empty) return null;

            var x = (rect.Left + rect.Right) / 2;
            var y = (rect.Top + rect.Bottom) / 2;

            return new Point(x, y);
        }


        void HighlightPoint(int x, int y)
        {
            // Ctrlキーを押すとマウスカーソルを強調表示するように設定してれば、これで強調されるはず
            // キーボードによって挙動が変わる？
            SendKeys.SendWait("^");
            Thread.Sleep(800);
        }

        // クリック系のテンプレート関数
        void click(int x, int y, MouseEventFlags flg1, MouseEventFlags flg2, int cnt)
        {
            // マウスクリックの前後、少し待たないと後続のマウス・キー入力がうまく行われない。なんで？
            Thread.Sleep(30);

            mouseMove(x, y);

            // スローモーション実行ならクリックする先を強調する
            if (slowPlayFlag)
            {
                HighlightPoint(x, y);
                Cursor.Position = new Point(x, y);
            }

            for (int i = 0; i < cnt; i++)
            {
                mouse_event((UInt32)flg1, (uint)x, (uint)y, 0, IntPtr.Zero);
                mouse_event((UInt32)flg2, (uint)x, (uint)y, 0, IntPtr.Zero);
            }

            // マウスクリックの前後、少し待たないと後続のマウス・キー入力がうまく行われない。なんで？
            Thread.Sleep(30);
        }

        // クリック系のテンプレート関数
        void click(string imageName, MouseEventFlags flg1, MouseEventFlags flg2, int cnt, double threshold)
        {
            var pos = findTemplatePoint(imageName, threshold);
            if (pos == null) return;
            click(pos.Value.X, pos.Value.Y, flg1, flg2, cnt);
        }

        //-----------------------------------------------------------------------------------------
        //
        // 公開メソッドたち
        //
        //-----------------------------------------------------------------------------------------
        
        // マウスカーソルの位置を取得
        public Point CursorPosition { get { return Cursor.Position; } }

        // 左クリック
        public void leftClick(int x, int y) { click(x, y, MouseEventFlags.LEFTDOWN, MouseEventFlags.LEFTUP, 1); }
        public void leftClick(Rectangle rect) { leftClick(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void leftClick(string imageName, double threshold = -1) { click(imageName, MouseEventFlags.LEFTDOWN, MouseEventFlags.LEFTUP, 1, threshold); }

        // 右クリック
        public void rightClick(int x, int y) { click(x, y, MouseEventFlags.RIGHTDOWN, MouseEventFlags.RIGHTUP, 1); }
        public void rightClick(Rectangle rect) { rightClick(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void rightClick(string imageName, double threshold = -1) { click(imageName, MouseEventFlags.RIGHTDOWN, MouseEventFlags.RIGHTUP, 1, threshold); }

        // ダブルクリック
        public void doubleClick(int x, int y) { click(x, y, MouseEventFlags.LEFTDOWN, MouseEventFlags.LEFTUP, 2); }
        public void doubleClick(Rectangle rect) { doubleClick(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void doubleClick(string imageName, double threshold = -1) { click(imageName, MouseEventFlags.LEFTDOWN, MouseEventFlags.LEFTUP, 2, threshold); }

        /// マウスカーソルを移動させる
        public void mouseMove(int x, int y)
        {
            double v = 1000;
            var sw = new Stopwatch();

            sw.Start();

            while (slowPlayFlag)
            {
                var sx = Cursor.Position.X;
                var sy = Cursor.Position.Y;
                double dx = x - sx;
                double dy = y - sy;
                sw.Stop();
                var length = Math.Sqrt(dx * dx + dy * dy);
                if (length <= 30) break;
                var dd = v * sw.Elapsed.TotalSeconds / length;
                sw.Restart();
                dx *= dd;
                dy *= dd;
                Cursor.Position = new Point(sx + (int)dx, sy + (int)dy);
                Thread.Sleep(30);  // 気持ち30fpsくらい
            }

            Cursor.Position = new Point(x, y);
        }
        public void mouseMove(Rectangle rect) { mouseMove(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void mouseMove(string imageName, double threshold = -1)
        {
            var pos = findTemplatePoint(imageName, threshold); if (pos == null) return;
            mouseMove(pos.Value.X, pos.Value.Y);
        }

        // ドラッグ
        public void drag(int x, int y)
        {

            // マウスクリックの前後、少し待たないと後続のマウス・キー入力がうまく行われない。なんで？
            Thread.Sleep(30);

            mouseMove(x, y);

            // スローモーション実行ならクリックする先を強調する
            if (slowPlayFlag)
            {
                HighlightPoint(x, y);
                Cursor.Position = new Point(x, y);
            }
            {
                mouse_event((UInt32)MouseEventFlags.LEFTDOWN, (uint)x, (uint)y, 0, IntPtr.Zero);
            }

            // マウスクリックの前後、少し待たないと後続のマウス・キー入力がうまく行われない。なんで？
            Thread.Sleep(30);    
            //click(x, y, MouseEventFlags.LEFTDOWN, MouseEventFlags.LEFTDOWN, 1);
        }
        public void drag(Rectangle rect) { drag(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void drag(string imageName, double threshold = -1) { click(imageName, MouseEventFlags.LEFTDOWN, MouseEventFlags.LEFTDOWN, 1, threshold); }

        // ドロップ
        public void drop(int x, int y) { click(x, y, MouseEventFlags.LEFTUP, MouseEventFlags.LEFTUP, 1); }
        public void drop(Rectangle rect) { drop(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void drop(string imageName, double threshold = -1) { click(imageName, MouseEventFlags.LEFTUP, MouseEventFlags.LEFTUP, 1, threshold); }

        // ドラッグアンドドロップ
        public void dragAndDrop(int x1, int y1, int x2, int y2)
        {
            // マウスカーソルを(x1, y1)にセット
            mouseMove(x1, y1);
            Thread.Sleep(30);

            // 左ボタンを押す
            mouse_event((UInt32)MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(30);

            // マウスカーソルを(x2, y2)の移動
            mouseMove(x2, y2);
            Thread.Sleep(30);

            // 左ボタンを離す
            mouse_event((UInt32)MouseEventFlags.LEFTUP, 0, 0, 0, IntPtr.Zero);
            Thread.Sleep(30);
        }

        public void dragAndDrop(Rectangle rect1, Rectangle rect2)
        {
            dragAndDrop(rect1.X + rect1.Width / 2, rect1.Y + rect1.Height / 2, rect2.X + rect2.Width / 2, rect2.Y + rect2.Height / 2);
        }

        public void dragAndDrop(string imageName1, string imageName2, double threshold1 = -1, double threshold2 = -1)
        {
            var pos1 = findTemplatePoint(imageName1, threshold1); if (pos1 == null) return;
            var pos2 = findTemplatePoint(imageName2, threshold2); if (pos2 == null) return;
            dragAndDrop(pos1.Value.X, pos1.Value.Y, pos2.Value.X, pos2.Value.Y);
        }

        //-----------------------------------------------------------------------------------------

        // キー入力
        public void type(string text)
        {
            Thread.Sleep(30); 
            SendKeys.SendWait(text);
            Thread.Sleep(30);
        }
        public void type(int x, int y, string text) { leftClick(x, y); type(text); }
        public void type(Rectangle rect, string text) { type(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, text); }
        public void type(string imageName, string text, double threshold = -1) { leftClick(imageName, threshold); type(text); }

        // 貼り付け
        public void paste(string text) {
            Thread.Sleep(30);
            owner.Invoke((Action)(() => Clipboard.SetText(text))); type("^v");
            Thread.Sleep(30);
        }
        public void paste(int x, int y, string text) { leftClick(x, y); paste(text); }
        public void paste(Rectangle rect, string text) { paste(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, text); }
        public void paste(string imageName, string text, double threshold = -1) { leftClick(imageName, threshold); paste(text); }

        //-----------------------------------------------------------------------------------------

        // 画像が見つかるまで待機（ポーリング）
        // timeoutが負数だったらタイムアウトしない
        public Rectangle wait(string imageName, int timeout = 100, double threshold = -1)
        {
            timeout *= 1000;
            var sw = new Stopwatch();
            var rect = Rectangle.Empty;
            while (timeout < 0 || sw.ElapsedMilliseconds < timeout)
            {
                sw.Start();
                // 見つかったら脱出
                if ((rect = owner.findTemplate(imageName, threshold, false)) != Rectangle.Empty) break;
                sw.Stop();
            }
            if (rect == Rectangle.Empty) MessageBox.Show("画像 " + imageName + " はスクリーン内で見つかりませんでした。");
            return rect;
        }

        // 画像がスクリーンから消えるまで待機（ポーリング）
        public void waitVanish(string imageName, int timeout = 100, double threshold = -1)
        {
            timeout *= 1000;
            var sw = new Stopwatch();
            while (sw.ElapsedMilliseconds < timeout)
            {
                sw.Start();
                // 見つからなかったら脱出
                if (owner.findTemplate(imageName, threshold, false) == Rectangle.Empty) return;
                sw.Stop();
            }
            MessageBox.Show("画像 " + imageName + " はスクリーンから消えませんでした");
        }

        //-----------------------------------------------------------------------------------------
        
        // 指定された画像が存在するか
        public bool exist(string imageName, double threshold = -1) { return owner.findTemplate(imageName, threshold, false) != Rectangle.Empty; }
        public Rectangle find(string imageName, double threshold = -1) { return owner.findTemplate(imageName, threshold, false); }

        //-----------------------------------------------------------------------------------------

        // スクリーン上の指定された領域のスクリーンショットを取り、その画像名を返す
        // imageNameで撮影した画像名を指定できる。既存の画像を上書きしたいときに使う
        public string capture(Rectangle rect, string imageName = null) { return owner.TakeAndAddScreenshot(rect, imageName); }
        public string capture(int x, int y, int w, int h, string imageName = null) { return capture(new Rectangle(x, y, w ,h), imageName); }

        //-----------------------------------------------------------------------------------------

        // メッセージボックスの表示
        public void messageBox(string text) { MessageBox.Show(text); }
    }
}
