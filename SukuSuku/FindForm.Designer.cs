namespace SukuSuku
{
    partial class FindForm
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
            this.checkBoxToTop = new System.Windows.Forms.CheckBox();
            this.textBoxReplace = new System.Windows.Forms.TextBox();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonUp = new System.Windows.Forms.RadioButton();
            this.radioButtonDown = new System.Windows.Forms.RadioButton();
            this.buttonReplaceAll = new System.Windows.Forms.Button();
            this.checkBoxCase = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonFind = new System.Windows.Forms.Button();
            this.textBoxFind = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxToTop
            // 
            this.checkBoxToTop.AutoSize = true;
            this.checkBoxToTop.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxToTop.Location = new System.Drawing.Point(33, 205);
            this.checkBoxToTop.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxToTop.Name = "checkBoxToTop";
            this.checkBoxToTop.Size = new System.Drawing.Size(260, 21);
            this.checkBoxToTop.TabIndex = 24;
            this.checkBoxToTop.Text = "文末まできたら文頭に移動する(&M)";
            this.checkBoxToTop.UseVisualStyleBackColor = true;
            // 
            // textBoxReplace
            // 
            this.textBoxReplace.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxReplace.Location = new System.Drawing.Point(205, 66);
            this.textBoxReplace.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxReplace.Name = "textBoxReplace";
            this.textBoxReplace.Size = new System.Drawing.Size(345, 24);
            this.textBoxReplace.TabIndex = 17;
            this.textBoxReplace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxReplace_KeyDown);
            // 
            // buttonReplace
            // 
            this.buttonReplace.Enabled = false;
            this.buttonReplace.Location = new System.Drawing.Point(560, 63);
            this.buttonReplace.Margin = new System.Windows.Forms.Padding(4);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(123, 29);
            this.buttonReplace.TabIndex = 19;
            this.buttonReplace.Text = "置換(&R)";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonUp);
            this.groupBox1.Controls.Add(this.radioButtonDown);
            this.groupBox1.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.Location = new System.Drawing.Point(357, 153);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(135, 74);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索する方法";
            // 
            // radioButtonUp
            // 
            this.radioButtonUp.AutoSize = true;
            this.radioButtonUp.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButtonUp.Location = new System.Drawing.Point(24, 22);
            this.radioButtonUp.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonUp.Name = "radioButtonUp";
            this.radioButtonUp.Size = new System.Drawing.Size(93, 21);
            this.radioButtonUp.TabIndex = 9;
            this.radioButtonUp.Text = "上に検索";
            this.radioButtonUp.UseVisualStyleBackColor = true;
            // 
            // radioButtonDown
            // 
            this.radioButtonDown.AutoSize = true;
            this.radioButtonDown.Checked = true;
            this.radioButtonDown.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButtonDown.Location = new System.Drawing.Point(24, 46);
            this.radioButtonDown.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonDown.Name = "radioButtonDown";
            this.radioButtonDown.Size = new System.Drawing.Size(93, 21);
            this.radioButtonDown.TabIndex = 10;
            this.radioButtonDown.TabStop = true;
            this.radioButtonDown.Text = "下に検索";
            this.radioButtonDown.UseVisualStyleBackColor = true;
            // 
            // buttonReplaceAll
            // 
            this.buttonReplaceAll.Enabled = false;
            this.buttonReplaceAll.Location = new System.Drawing.Point(560, 99);
            this.buttonReplaceAll.Margin = new System.Windows.Forms.Padding(4);
            this.buttonReplaceAll.Name = "buttonReplaceAll";
            this.buttonReplaceAll.Size = new System.Drawing.Size(123, 29);
            this.buttonReplaceAll.TabIndex = 20;
            this.buttonReplaceAll.Text = "すべて置換(&A)";
            this.buttonReplaceAll.UseVisualStyleBackColor = true;
            this.buttonReplaceAll.Click += new System.EventHandler(this.buttonReplaceAll_Click);
            // 
            // checkBoxCase
            // 
            this.checkBoxCase.AutoSize = true;
            this.checkBoxCase.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxCase.Location = new System.Drawing.Point(33, 155);
            this.checkBoxCase.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxCase.Name = "checkBoxCase";
            this.checkBoxCase.Size = new System.Drawing.Size(236, 21);
            this.checkBoxCase.TabIndex = 22;
            this.checkBoxCase.Text = "大文字・小文字を区別する(&C)";
            this.checkBoxCase.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 11.25F);
            this.label2.Location = new System.Drawing.Point(29, 73);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 19);
            this.label2.TabIndex = 15;
            this.label2.Text = "置換後の文字列(&P)";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(560, 198);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(123, 29);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonFind
            // 
            this.buttonFind.Enabled = false;
            this.buttonFind.Location = new System.Drawing.Point(560, 30);
            this.buttonFind.Margin = new System.Windows.Forms.Padding(4);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(123, 29);
            this.buttonFind.TabIndex = 18;
            this.buttonFind.Text = "次を検索(&F)";
            this.buttonFind.UseVisualStyleBackColor = true;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            // 
            // textBoxFind
            // 
            this.textBoxFind.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxFind.Location = new System.Drawing.Point(205, 34);
            this.textBoxFind.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxFind.Name = "textBoxFind";
            this.textBoxFind.Size = new System.Drawing.Size(345, 24);
            this.textBoxFind.TabIndex = 16;
            this.textBoxFind.TextChanged += new System.EventHandler(this.textBoxFind_TextChanged);
            this.textBoxFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFind_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F);
            this.label1.Location = new System.Drawing.Point(29, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 19);
            this.label1.TabIndex = 14;
            this.label1.Text = "検索する文字列(&N)";
            // 
            // FindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 274);
            this.Controls.Add(this.checkBoxToTop);
            this.Controls.Add(this.textBoxReplace);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonReplaceAll);
            this.Controls.Add(this.checkBoxCase);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonFind);
            this.Controls.Add(this.textBoxFind);
            this.Controls.Add(this.label1);
            this.Name = "FindForm";
            this.Text = "FindForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBoxToTop;
        private System.Windows.Forms.TextBox textBoxReplace;
        private System.Windows.Forms.Button buttonReplace;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonUp;
        private System.Windows.Forms.RadioButton radioButtonDown;
        private System.Windows.Forms.Button buttonReplaceAll;
        private System.Windows.Forms.CheckBox checkBoxCase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.TextBox textBoxFind;
        private System.Windows.Forms.Label label1;

    }
}