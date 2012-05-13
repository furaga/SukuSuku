using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using OpenCvSharp;
using Sgry.Azuki;
using Sgry.Azuki.Highlighter;

namespace SukuSuku
{
    public partial class MainForm : Form
    {
        Microsoft.Scripting.Hosting.ScriptEngine engine;
        Microsoft.Scripting.Hosting.ScriptScope scope;
        System.Threading.Thread thread;
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
            textBox.Image = templateBMPs;
            engine = IronRuby.Ruby.CreateEngine();
            ui = new UI(this);
            scope = engine.CreateScope();
            scope.SetVariable("ui", ui);

        }

        //----------------------------------------------------------------------
        // ファイル
        //----------------------------------------------------------------------

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

        //----------------------------------------------------------------------
        // 編集
        //----------------------------------------------------------------------

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

        BlackForm blackForm = null;

        private void screenshotButton_Click(object sender, EventArgs e)
        {
            blackForm = new BlackForm();
            blackForm.Show(this);
        }
        //----------------------------------------------------------------------
        // 実行
        //----------------------------------------------------------------------

        private void 実行RToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // 通常実行
            ui.slowPlayFlag = false;
            Run();
        }


        private void スローモーションで実行RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // スローモーション実行
            ui.slowPlayFlag = true;
            Run();
        }

        private void 停止SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 実行中のスレッドを殺す
            if (thread != null && thread.IsAlive) thread.Abort();
            SetPlayButtons(false);
            toolStripStatusLabel.Text = "強制終了";
        }

        //----------------------------------------------------------------------
        // 表示
        //----------------------------------------------------------------------

        private void フォントの設定FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fontDialog.Font = fontEditor;
                if (fontDialog.ShowDialog(this) == DialogResult.OK)
                {
                    SetFontEditor(fontDialog.Font);
                    デフォルトのフォントToolStripMenuItem.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        private void デフォルトのフォントToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFontEditor(デフォルトのフォントToolStripMenuItem.Checked ? defaultFontEditor : fontDialog.Font);
        }

        private void すくすくについてAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutForm().Show();
        }

        private void cmdButton_Click(object sender, EventArgs e)
        {
            var text = ((Button)sender).Text;

            if (autoChapCheckBox.Checked)
            {
                var res = new System.Text.RegularExpressions.Regex(@"(image\d*)").Matches(text);
                foreach (System.Text.RegularExpressions.Match m in res)
                {
                    var imageName = new BlackForm().takeScreenshot(this);
                    if (imageName == "") return;
                    text = text.Replace(m.Value, String.Format("\"{0}\"", imageName));
                }
            }

            textBox.Document.Replace(text + "\n");
        }

        private void thumbNailView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox.Document.Replace('"' + thumbNailView.SelectedItems[0].Text + '"');
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            setTextBoxImages();
        }

        private void 撮り直しRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (thumbNailView.SelectedItems.Count <= 0) return;
            var targetName = thumbNailView.SelectedItems[0].Text;
            var imageName = new BlackForm().takeScreenshot(this);
            var bmp = DeleteScreenshot(imageName);
            if (bmp == null) return;
            AddScreenshot(targetName, bmp);
            textBox.Refresh();
        }

        private void 削除DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (thumbNailView.SelectedItems.Count <= 0) return;
            var targetName = thumbNailView.SelectedItems[0].Text;
            DeleteScreenshot(targetName);
            textBox.Refresh();
        }

        FindForm findDialog = new FindForm();
        private void 検索FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findDialog.TargetTextBox = textBox;
            findDialog.FindString = textBox.GetSelectedText();
            findDialog.Hide();
            findDialog.Show(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (thread != null && thread.IsAlive)
            {
                e.Cancel = true;
                MessageBox.Show("実行を停止してからウインドウを閉じてください");
            }

            if (notifyIcon.Visible)
            {
                //e.Cancel = true;
                //Hide();
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            Activate();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon.Visible = false;
        }
    }
}
