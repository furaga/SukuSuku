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
        Microsoft.Scripting.Hosting.ScriptEngine engine;
        UI ui;

        public MainForm()
        {
            InitializeComponent();
        }

        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            textBox.Highlighter = Highlighters.Ruby;
            engine = IronRuby.Ruby.CreateEngine();
            ui = new UI(this);
        }

        private void 実行RToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                var scope = engine.CreateScope();
                scope.SetVariable("ui", ui);
                engine.Execute(textBox.Text, scope);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void commandView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        BlackForm blackForm = null;

        private void screenshotButton_Click(object sender, EventArgs e)
        {
            blackForm = new BlackForm();
            blackForm.Show(this);
        }

        public void SetCursor(int x, int y)
        {
            Cursor.Position = new Point(x, y);
        }

        private void もとに戻すUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Undo();
        }

        private void やり直しRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Redo();
        }

        private void 切り取りTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Cut();
        }

        private void コピーCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Copy();
        }

        private void 貼り付けPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste();
        }

        private void すべて選択AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.SelectAll();
        }

        private void 一行削除LToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int start, end;
            textBox.GetSelection(out start, out end);
            var beginRes = textBox.Document.FindPrev("\n", 0, start);
            var endRes = textBox.Document.FindNext("\n", end, textBox.TextLength);
            var beginPos = beginRes != null ? beginRes.End : 0;
            var endPos = endRes != null ? endRes.End : textBox.TextLength;
            textBox.Document.Replace("", beginPos, endPos);
        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
            SetTitle();
        }

        private void 名前をつけて保存AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(false);
            SetTitle();
        }

        private void 新規作成NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckSave()) NewFile();
            SetTitle();
        }

        private void 開くOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckSave()) OpenFile();
            SetTitle();
        }

    }
}
