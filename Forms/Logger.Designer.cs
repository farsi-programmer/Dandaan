﻿namespace Dandaan.Forms
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
            this.textBrowser1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.textBrowser1.Location = new System.Drawing.Point(0, 0);
            this.textBrowser1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBrowser1.Name = "textBrowser1";
            this.textBrowser1.Size = new System.Drawing.Size(784, 562);
            this.textBrowser1.TabIndex = 0;
            // 
            // Logger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.textBrowser1);
            this.Name = "Logger";
            this.Text = "لاگ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Logger_FormClosing);
            this.Load += new System.EventHandler(this.FormLogger_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private TextBrowser textBrowser1;
    }
}