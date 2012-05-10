using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sgry.Azuki;
using Sgry.Azuki.Windows;

namespace SukuSuku
{
    public partial class FindForm : Form
    {
        enum Mode { Find, Replace, ReplaceAll } 
        AzukiControl textBox;
        int findCount = 0;
        string dialogTitle = "";

        public AzukiControl TargetTextBox
        {
            set
            {
                textBox = value;
            }
        }

        public string FindString
        {
            set
            {
                textBoxFind.Text = value;
            }
        }

        public FindForm()
        {
            InitializeComponent();
        }

        private bool Find(Mode mode)
        {
            try
            {
                var selectStart = 0;
                var selectEnd = 0; 
                textBox.Document.GetSelection(out selectStart, out selectEnd);
                var position =
                    radioButtonUp.Checked ?
                    new { start = 0, end = selectStart, prevPos = selectStart } :
                    new { start = selectEnd, end = textBox.TextLength, prevPos = selectEnd };
                var option = checkBoxCase.Checked;
                var res = Find(position.start, position.end, position.prevPos, option);
                if (res == false)
                {
                    if (checkBoxToTop.Checked)
                    {
                        position =
                            radioButtonUp.Checked ?
                            new { start = selectStart, end = textBox.TextLength, prevPos = selectStart } :
                            new { start = 0, end = selectEnd, prevPos = selectEnd };
                        res = Find(position.start, position.end, position.prevPos, option);
                    }
                }
                if (res == false)
                {
                    var message = "";
                    switch (mode)
                    {
                        case Mode.Find:
                        case Mode.Replace:
                            message = (checkBoxToTop.Checked ? "検索の開始位置に達しました" : "ドキュメントの最後まで検索しました");
                            break;
                        default:
                            message = findCount + "件置換しました";
                            break;
                    }
                    MessageBox.Show(this, message, dialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return res;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool Find(int start, int end, int prevPos, bool matchCase)
        {
            try
            {
                var findString = textBoxFind.Text;
                var findStringLength = findString.Length;
                SearchResult res = null;
                if (radioButtonUp.Checked)
                {
                    res = textBox.Document.FindPrev(findString, start, end, matchCase);
                }
                else
                {
                    res = textBox.Document.FindNext(findString, start, end, matchCase);
                }
                if (res != null)
                {
                    textBox.SetSelection(res.Begin, res.End);
                    textBox.ScrollToCaret();
                    findCount++;
                    textBox.Focus();
                    return true;
                }
                textBox.SetSelection(prevPos, prevPos);
                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckEqualString()
        {
            try
            {
                StringComparison cmp = checkBoxCase.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                if (textBoxFind.Text.Equals(textBoxReplace.Text, cmp))
                {
                    MessageBox.Show(this, "同じ文字列で置換することはできません", dialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool EqualString(string s1, string s2)
        {
            try
            {
                StringComparison cmp = checkBoxCase.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                return s1.Equals(s2, cmp);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            Find(Mode.Find);
        }

        private void buttonReplace_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckEqualString()) return;
                if (EqualString(textBox.GetSelectedText(), textBoxFind.Text))
                {
                    textBox.Document.Replace(textBoxReplace.Text);
                }
                Find(Mode.Replace);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonReplaceAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckEqualString()) return;
                findCount = 0;
                while (Find(Mode.ReplaceAll))
                {
                    if (EqualString(textBox.GetSelectedText(), textBoxFind.Text))
                    {
                        textBox.Document.Replace(textBoxReplace.Text);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FindForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void textBoxFind_TextChanged(object sender, EventArgs e)
        {
            buttonFind.Enabled = buttonReplace.Enabled = buttonReplaceAll.Enabled = textBoxFind.Text != "";
        }

        private void textBoxFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (buttonFind.Enabled && e.KeyData == Keys.Enter)
            {
                Find(Mode.Find);
            }
        }

        private void textBoxReplace_KeyDown(object sender, KeyEventArgs e)
        {
            if (buttonReplace.Enabled && e.KeyData == Keys.Enter)
            {
                buttonReplace_Click(sender, e);
            }
        }
    }
}
