namespace Dandaan.Forms
{
    partial class Logger
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
            this.textBrowser1 = new Dandaan.Forms.TextBrowser();
            this.SuspendLayout();
            // 
            // textBrowser1
            // 
            this.textBrowser1.Name = "textBrowser1";
            this.textBrowser1.TabIndex = 0;
            // 
            // Logger
            // 
            this.Controls.Add(this.textBrowser1);
            this.Name = "Logger";
            this.Text = "لاگ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Logger_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private TextBrowser textBrowser1;
    }
}