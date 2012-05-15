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
    public partial class ShortcutForm : Form
    {
        MainForm owner = null;

        public ShortcutForm(MainForm owner)
        {
            InitializeComponent();
            this.owner = owner;
        }

        private void ShortcutForm_Load(object sender, EventArgs e)
        {
            keyComboBox.Items.AddRange(Enum.GetNames(typeof(Keys)).OrderBy(s => s).OrderBy(s => s.Length).ToArray());
            SetShortcutsDataGridView();
        }

        void SetShortcutsDataGridView()
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
                shortcutsDataGridView.Rows.Add(new[] { command, action.Value.Item1, "削除" });
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

        private void assignButton_Click(object sender, EventArgs e)
        {
            var modifier = AltCheckBox.Checked ? MainForm.MOD_ALT : 0;
            modifier |= CtrlCheckBox.Checked ? MainForm.MOD_CONTROL : 0;
            modifier |= WinCheckBox.Checked ? MainForm.MOD_WIN : 0;
            modifier |= ShiftCheckBox.Checked ? MainForm.MOD_SHIFT : 0;

            Keys key;
            Enum.TryParse(keyComboBox.Text, out key);

            owner.AddHotKeyActionByScript(modifier, key, scriptPathTextBox.Text);

            SetShortcutsDataGridView();
        }
    }
}