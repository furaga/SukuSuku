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
    /// <summary>
    /// ショートカットキー（ホットキー）の登録をするためのフォーム
    /// </summary>
    public partial class ShortcutForm : Form
    {
        const string configPath = "./config.conf";

        MainForm owner = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner"></param>
        public ShortcutForm(MainForm owner)
        {
            InitializeComponent();
            this.owner = owner;
        }

        /// <summary>
        /// フォームが開いた瞬間（？）に呼ばれる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortcutForm_Load(object sender, EventArgs e)
        {
            keyComboBox.Items.AddRange(Enum.GetNames(typeof(Keys)).OrderBy(s => s).OrderBy(s => s.Length).ToArray());

            LoadConfigData();

            SetShortcutsDataGridView();
        }

        /// <summary>
        /// 設定ファイル(configPath)の内容を読みこんでホットキーを適宜登録する
        /// </summary>
        void LoadConfigData()
        {
            foreach (var line in System.IO.File.ReadLines(configPath).Where(s => s.Trim().Length >= 1 && s.Trim()[0] != '#'))
            {
                var tokens = line.Split(new []{','}).Select(s => s.Trim()).ToArray();
                if (tokens.Length != 2) continue;
                uint id;
                if (uint.TryParse(tokens[0], out id) == false) continue;   // IDをパース
                if (tokens[1].EndsWith(".rb") == false || System.IO.File.Exists(tokens[1]) == false) continue; // ファイル名が不正でないか
                owner.AddHotKeyActionByScript(id & 0xffff, (Keys)(id / 0x10000), tokens[1]);
            }
        }

        /// <summary>
        /// shortcutsDataGridViewの内容をセット
        /// </summary>
        /// <param name="saveToConfigFile">コンフィグファイルにセットした内容を保存するか</param>
        void SetShortcutsDataGridView(bool saveToConfigFile = false)
        {
            using (var sw = new System.IO.StreamWriter(configPath, false))
            {
                shortcutsDataGridView.Rows.Clear();
                foreach (var action in owner.HotKeyActions)
                {
                    var modifiers = (uint)action.Key & 0xffff;
                    var key = (uint)action.Key / 0x10000;
                    var command = (modifiers & MainForm.MOD_CONTROL) != 0 ? "Ctrl + " : "";
                    command += (modifiers & MainForm.MOD_ALT) != 0 ? "Alt + " : "";
                    command += (modifiers & MainForm.MOD_SHIFT) != 0 ? "Shift + " : "";
                    command += (modifiers & MainForm.MOD_WIN) != 0 ? "Win + " : "";
                    command += ((Keys)key).ToString();
                    shortcutsDataGridView.Rows.Add(new[] { command, action.Value.Item1 });

                    if (saveToConfigFile) sw.WriteLine((uint)action.Key + "," + action.Value.Item1);
                }
            }
        }

        private void scriptPathButton_Click(object sender, EventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Rubyファイル(*.rb)|*.rb";
                dialog.RestoreDirectory = true;
                if (dialog.ShowDialog(this) == DialogResult.OK) scriptPathTextBox.Text = dialog.FileName;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        /// <summary>
        /// ホットキーを登録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void assignButton_Click(object sender, EventArgs e)
        {
            var modifier = AltCheckBox.Checked ? MainForm.MOD_ALT : 0;
            modifier |= CtrlCheckBox.Checked ? MainForm.MOD_CONTROL : 0;
            modifier |= WinCheckBox.Checked ? MainForm.MOD_WIN : 0;
            modifier |= ShiftCheckBox.Checked ? MainForm.MOD_SHIFT : 0;
            Keys key;
            if (Enum.TryParse(keyComboBox.Text, out key) == false) return;

            owner.AddHotKeyActionByScript(modifier, key, scriptPathTextBox.Text);

            SetShortcutsDataGridView(true);
        }
    }
}