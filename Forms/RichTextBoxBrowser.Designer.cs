﻿namespace Dandaan.Forms
{
    partial class RichTextBoxBrowser<T>
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
            this.richTextBoxBrowser1 = new Dandaan.UserControls.RichTextBoxBrowser();
            this.SuspendLayout();
            // 
            // richTextBoxBrowser1
            // 
            this.richTextBoxBrowser1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.richTextBoxBrowser1.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxBrowser1.Name = "richTextBoxBrowser1";
            this.richTextBoxBrowser1.Size = new System.Drawing.Size(784, 562);
            this.richTextBoxBrowser1.TabIndex = 0;
            // 
            // RichTextBoxBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.richTextBoxBrowser1);
            this.Name = "RichTextBoxBrowser";
            this.Text = "RichTextBoxBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.RichTextBoxBrowser richTextBoxBrowser1;
    }
}