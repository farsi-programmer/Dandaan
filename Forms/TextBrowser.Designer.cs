namespace Dandaan.Forms
{
    partial class TextBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextBrowser));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.browser1 = new Dandaan.Forms.Browser();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(760, 495);
            this.textBox1.TabIndex = 4;
            // 
            // browser1
            // 
            this.browser1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F);
            this.browser1.Location = new System.Drawing.Point(-5, 509);
            this.browser1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.browser1.Name = "browser1";
            this.browser1.Size = new System.Drawing.Size(800, 51);
            this.browser1.TabIndex = 5;
            // 
            // TextBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.browser1);
            this.Controls.Add(this.textBox1);
            this.Name = "TextBrowser";
            this.Size = new System.Drawing.Size(784, 562);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private Browser browser1;
    }
}
