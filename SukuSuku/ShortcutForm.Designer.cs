namespace SukuSuku
{
    partial class ShortcutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShortcutForm));
            this.CtrlCheckBox = new System.Windows.Forms.CheckBox();
            this.AltCheckBox = new System.Windows.Forms.CheckBox();
            this.WinCheckBox = new System.Windows.Forms.CheckBox();
            this.ShiftCheckBox = new System.Windows.Forms.CheckBox();
            this.keyComboBox = new System.Windows.Forms.ComboBox();
            this.scriptPathTextBox = new System.Windows.Forms.TextBox();
            this.scriptPathButton = new System.Windows.Forms.Button();
            this.assignButton = new System.Windows.Forms.Button();
            this.shortcutsDataGridView = new System.Windows.Forms.DataGridView();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Scripts = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.shortcutsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // CtrlCheckBox
            // 
            this.CtrlCheckBox.AutoSize = true;
            this.CtrlCheckBox.Location = new System.Drawing.Point(37, 52);
            this.CtrlCheckBox.Name = "CtrlCheckBox";
            this.CtrlCheckBox.Size = new System.Drawing.Size(52, 19);
            this.CtrlCheckBox.TabIndex = 0;
            this.CtrlCheckBox.Text = "Ctrl";
            this.CtrlCheckBox.UseVisualStyleBackColor = true;
            // 
            // AltCheckBox
            // 
            this.AltCheckBox.AutoSize = true;
            this.AltCheckBox.Location = new System.Drawing.Point(118, 52);
            this.AltCheckBox.Name = "AltCheckBox";
            this.AltCheckBox.Size = new System.Drawing.Size(46, 19);
            this.AltCheckBox.TabIndex = 1;
            this.AltCheckBox.Text = "Alt";
            this.AltCheckBox.UseVisualStyleBackColor = true;
            // 
            // WinCheckBox
            // 
            this.WinCheckBox.AutoSize = true;
            this.WinCheckBox.Location = new System.Drawing.Point(198, 52);
            this.WinCheckBox.Name = "WinCheckBox";
            this.WinCheckBox.Size = new System.Drawing.Size(51, 19);
            this.WinCheckBox.TabIndex = 2;
            this.WinCheckBox.Text = "Win";
            this.WinCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShiftCheckBox
            // 
            this.ShiftCheckBox.AutoSize = true;
            this.ShiftCheckBox.Location = new System.Drawing.Point(267, 52);
            this.ShiftCheckBox.Name = "ShiftCheckBox";
            this.ShiftCheckBox.Size = new System.Drawing.Size(59, 19);
            this.ShiftCheckBox.TabIndex = 3;
            this.ShiftCheckBox.Text = "Shift";
            this.ShiftCheckBox.UseVisualStyleBackColor = true;
            // 
            // keyComboBox
            // 
            this.keyComboBox.FormattingEnabled = true;
            this.keyComboBox.Location = new System.Drawing.Point(332, 48);
            this.keyComboBox.Name = "keyComboBox";
            this.keyComboBox.Size = new System.Drawing.Size(235, 23);
            this.keyComboBox.TabIndex = 4;
            // 
            // scriptPathTextBox
            // 
            this.scriptPathTextBox.Location = new System.Drawing.Point(37, 13);
            this.scriptPathTextBox.Name = "scriptPathTextBox";
            this.scriptPathTextBox.Size = new System.Drawing.Size(497, 22);
            this.scriptPathTextBox.TabIndex = 5;
            // 
            // scriptPathButton
            // 
            this.scriptPathButton.Location = new System.Drawing.Point(540, 13);
            this.scriptPathButton.Name = "scriptPathButton";
            this.scriptPathButton.Size = new System.Drawing.Size(27, 23);
            this.scriptPathButton.TabIndex = 6;
            this.scriptPathButton.Text = "...";
            this.scriptPathButton.UseVisualStyleBackColor = true;
            this.scriptPathButton.Click += new System.EventHandler(this.scriptPathButton_Click);
            // 
            // assignButton
            // 
            this.assignButton.Location = new System.Drawing.Point(477, 90);
            this.assignButton.Name = "assignButton";
            this.assignButton.Size = new System.Drawing.Size(90, 23);
            this.assignButton.TabIndex = 7;
            this.assignButton.Text = "新規登録";
            this.assignButton.UseVisualStyleBackColor = true;
            this.assignButton.Click += new System.EventHandler(this.assignButton_Click);
            // 
            // shortcutsDataGridView
            // 
            this.shortcutsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.shortcutsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Command,
            this.Scripts});
            this.shortcutsDataGridView.Location = new System.Drawing.Point(26, 138);
            this.shortcutsDataGridView.Name = "shortcutsDataGridView";
            this.shortcutsDataGridView.RowTemplate.Height = 24;
            this.shortcutsDataGridView.Size = new System.Drawing.Size(844, 398);
            this.shortcutsDataGridView.TabIndex = 8;
            // 
            // Command
            // 
            this.Command.HeaderText = "コマンド";
            this.Command.Name = "Command";
            this.Command.ReadOnly = true;
            this.Command.Width = 200;
            // 
            // Scripts
            // 
            this.Scripts.HeaderText = "スクリプト";
            this.Scripts.Name = "Scripts";
            this.Scripts.ReadOnly = true;
            this.Scripts.Width = 600;
            // 
            // ShortcutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 565);
            this.Controls.Add(this.shortcutsDataGridView);
            this.Controls.Add(this.assignButton);
            this.Controls.Add(this.scriptPathButton);
            this.Controls.Add(this.scriptPathTextBox);
            this.Controls.Add(this.keyComboBox);
            this.Controls.Add(this.ShiftCheckBox);
            this.Controls.Add(this.WinCheckBox);
            this.Controls.Add(this.AltCheckBox);
            this.Controls.Add(this.CtrlCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ShortcutForm";
            this.Text = "ShortcutForm";
            this.Load += new System.EventHandler(this.ShortcutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.shortcutsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CtrlCheckBox;
        private System.Windows.Forms.CheckBox AltCheckBox;
        private System.Windows.Forms.CheckBox WinCheckBox;
        private System.Windows.Forms.CheckBox ShiftCheckBox;
        private System.Windows.Forms.ComboBox keyComboBox;
        private System.Windows.Forms.TextBox scriptPathTextBox;
        private System.Windows.Forms.Button scriptPathButton;
        private System.Windows.Forms.Button assignButton;
        private System.Windows.Forms.DataGridView shortcutsDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Scripts;
    }
}