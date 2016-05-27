namespace Dandaan.Forms
{
    partial class ListViewBrowser<T>
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
            this.listViewBrowser1 = new Dandaan.UserControls.ListViewBrowser();
            this.SuspendLayout();
            // 
            // listViewBrowser1
            // 
            this.listViewBrowser1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.listViewBrowser1.Location = new System.Drawing.Point(0, 0);
            this.listViewBrowser1.Name = "listViewBrowser1";
            this.listViewBrowser1.Size = new System.Drawing.Size(784, 562);
            this.listViewBrowser1.TabIndex = 0;
            // 
            // ListViewBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.listViewBrowser1);
            this.Name = "ListViewBrowser";
            this.Text = "ListViewBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.ListViewBrowser listViewBrowser1;
    }
}