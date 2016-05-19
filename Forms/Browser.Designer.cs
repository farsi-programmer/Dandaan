namespace Dandaan.Forms
{
    partial class Browser
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
            this.browserMenu1 = new Dandaan.Forms.BrowserMenu();
            this.SuspendLayout();
            // 
            // browserMenu1
            // 
            this.browserMenu1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.browserMenu1.Location = new System.Drawing.Point(0, 0);
            this.browserMenu1.Name = "browserMenu1";
            this.browserMenu1.Size = new System.Drawing.Size(784, 562);
            this.browserMenu1.TabIndex = 0;
            // 
            // Browser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.browserMenu1);
            this.Name = "Browser";
            this.ResumeLayout(false);

        }

        #endregion

        public BrowserMenu browserMenu1;
    }
}
