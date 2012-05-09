using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OpenCvSharp;

namespace SukuSuku
{
    // クリックやキー入力などのGUI操作を行う
    // スクリプト側ではこの中の関数が呼ばれることでGUI操作を行う
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

        // コンスタラクタ
        public UI(MainForm owner)
        {
            this.owner = owner;
        }

        // MainForm.findTemplate()で帰ってきた矩形の中心座標を返す(Rectangle.Emptyだったらnullを返す)
        private Point? findTemplatePoint(string imageName, double threshold)
        {
            //var rect = owner.findTemplate(imageName, threshold);

            // 見つかるまで待つ
            var rect = wait(imageName, 100, threshold);
            if (rect == Rectangle.Empty) return null;

            var x = (rect.Left + rect.Right) / 2;
            var y = (rect.Top + rect.Bottom) / 2;

            return new Point(x, y);
        }

        // クリック系のテンプレート関数
        private void click(int x, int y, MouseEventFlags flg1, MouseEventFlags flg2, int cnt)
        {
            // マウスクリックの前後、少し待たないと後続のマウス・キー入力がうまく行われない。なんでじゃろ？
            System.Threading.Thread.Sleep(100);

            Cursor.Position = new Point(x, y);

            for (int i = 0; i < cnt; i++)
            {
                mouse_event((UInt32)flg1, (uint)x, (uint)y, 0, IntPtr.Zero);
                mouse_event((UInt32)flg2, (uint)x, (uint)y, 0, IntPtr.Zero);
            }

            // マウスクリックの前後、少し待たないと後続のマウス・キー入力がうまく行われない。なんでじゃろ？
            System.Threading.Thread.Sleep(100);
        }

        // クリック系のテンプレート関数
        private void click(string imageName, MouseEventFlags flg1, MouseEventFlags flg2, int cnt, double threshold)
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

        // カーソル移動（無駄に2回同じ所に移動させてるけど特に問題ないはず）
        public void mouseMove(int x, int y) { click(x, y, MouseEventFlags.ABSOLUTE, MouseEventFlags.ABSOLUTE, 1); }
        public void mouseMove(Rectangle rect) { mouseMove(rect.X + rect.Width / 2, rect.Y + rect.Height / 2); }
        public void mouseMove(string imageName, double threshold = -1) { click(imageName, MouseEventFlags.ABSOLUTE, MouseEventFlags.ABSOLUTE, 1, threshold); }

        // キー入力
        public void type(string text) { SendKeys.SendWait(text); }
        public void type(int x, int y, string text) { leftClick(x, y); type(text); }
        public void type(Rectangle rect, string text) { type(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, text); }
        public void type(string imageName, string text, double threshold = -1) { leftClick(imageName, threshold); type(text); }

        // 貼り付け
        public void paste(string text) { Clipboard.SetText(text); type("^v"); }
        public void paste(int x, int y, string text) { leftClick(x, y); paste(text); }
        public void paste(Rectangle rect, string text) { paste(rect.X + rect.Width / 2, rect.Y + rect.Height / 2, text); }
        public void paste(string imageName, string text, double threshold = -1) { leftClick(imageName, threshold); paste(text); }

        // 指定された画像が存在するか
        public bool exist(string imageName, double threshold = -1) { return owner.findTemplate(imageName, threshold, false) != Rectangle.Empty; }

        // 画像が見つかるまで待機（ポーリング）
        public Rectangle wait(string imageName, int timeout = 100, double threshold = -1)
        {
            timeout *= 1000;
            var sw = new Stopwatch();
            var rect = Rectangle.Empty;
            while (sw.ElapsedMilliseconds < timeout)
            {
                sw.Start();
                // 見つかったら脱出
                if ((rect = owner.findTemplate(imageName, threshold, false)) != Rectangle.Empty) break;
                sw.Stop();
            }
            if (rect == Rectangle.Empty) MessageBox.Show(owner, "画像 " + imageName + " はスクリーン内で見つかりませんでした。");
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
            MessageBox.Show(owner, "画像 " + imageName + " はスクリーンから消えませんでした");
        }

        // ドラッグアンドドロップ
        public void dragAndDrop(int x1, int y1, int x2, int y2)
        {
            // マウスカーソルを(x1, y1)にセット
            Cursor.Position = new Point(x1, y1);
            System.Threading.Thread.Sleep(100);

            // 左ボタンを押す
            mouse_event((UInt32)MouseEventFlags.LEFTDOWN, 0, 0, 0, IntPtr.Zero);
            System.Threading.Thread.Sleep(100);

            // マウスカーソルを(x2, y2)の移動
            Cursor.Position = new Point(x2, y2);
            System.Threading.Thread.Sleep(100);
            
            // 左ボタンを離す
            mouse_event((UInt32)MouseEventFlags.LEFTUP, 0, 0, 0, IntPtr.Zero);
            System.Threading.Thread.Sleep(100);
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
    }
}
