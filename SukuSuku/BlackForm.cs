using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SukuSuku
{
    public partial class BlackForm : Form
    {
        Pen pen = new Pen(Color.Red, 2);
        Point? start = null;
        Point? end = null;

        public BlackForm()
        {
            InitializeComponent();
        }

        private void BlackForm_Load(object sender, EventArgs e)
        {
            start = end = null;

            if (WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            this.WindowState = FormWindowState.Maximized;
        }

        private Rectangle GetRectangle(Point start, Point end)
        {
            var x = Math.Min(start.X, end.X);
            var y = Math.Min(start.Y, end.Y);
            var w = Math.Abs(start.X - end.X);
            var h = Math.Abs(start.Y - end.Y);
            return new Rectangle(x, y, w, h);
        }

        private void BlackForm_MouseDown(object sender, MouseEventArgs e)
        {
            start = MousePosition;
        }

        private void BlackForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (start != null && end != null)
            {
                var rect = GetRectangle(start.Value, end.Value);
                if (rect.Width != 0 && rect.Height != 0)
                {
                    this.Opacity = 0;
                    __imageName = ((MainForm)Owner).TakeAndAddScreenshot(rect);
                }
            }
            Hide();
        }

        private void BlackForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (start == null) return;

            end = MousePosition;

            using (var g = CreateGraphics())
            {
                g.Clear(Color.Black);
                var rect = GetRectangle(start.Value, end.Value);
                g.DrawRectangle(pen, rect);

                var pt1 = new Point(rect.Left, (rect.Top + rect.Bottom) / 2);
                var pt2 = new Point(rect.Right, pt1.Y);
                g.DrawLine(pen, pt1, pt2);
                
                pt1 = new Point((rect.Left + rect.Right) / 2, rect.Top);
                pt2 = new Point(pt1.X, rect.Bottom);
                g.DrawLine(pen, pt1, pt2);
            }
        }

        private string __imageName = "";
        private Bitmap __screenshot = null;
        public string takeScreenshot(MainForm owner)
        {
            var visible = owner.Visible;
            __screenshot = owner.TakeScreenshot(Screen.PrimaryScreen.Bounds);
            using (var form = new Form())
            {
                if (form.WindowState == FormWindowState.Maximized)
                {
                    form.WindowState = FormWindowState.Normal;
                }
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
                form.Paint += (sender, e) => e.Graphics.DrawImage(__screenshot, Point.Empty);
                form.Show();
                form.Activate();

                owner.Hide();
                ShowDialog(owner);
            }
            if (visible) owner.Show();

            return __imageName; 
        }
    }
}
