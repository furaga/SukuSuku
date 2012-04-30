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

    }
}
