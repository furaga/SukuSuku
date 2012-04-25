namespace SukuSuku
{
    partial class BlackForm
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
            this.SuspendLayout();
            // 
            // BlackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(282, 255);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "BlackForm";
            this.Opacity = 0.5D;
            this.Text = "BlackForm";
            this.Load += new System.EventHandler(this.BlackForm_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BlackForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BlackForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BlackForm_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}