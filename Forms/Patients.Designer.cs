namespace Dandaan.Forms
{
    partial class Patients
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
            //this.listBrowser1 = new Dandaan.Forms.ListBoxBrowser();
            this.listBrowser1 = new Dandaan.Forms.CheckedListBrowser();
            this.SuspendLayout();
            // 
            // listBrowser1
            // 
            this.listBrowser1.Name = "listBrowser1";
            this.listBrowser1.TabIndex = 1;
            // 
            // Patients
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.listBrowser1);
            this.Name = "Patients";
            this.Text = "بیماران";
            this.Load += new System.EventHandler(this.Patients_Load);
            this.ResizeBegin += new System.EventHandler(this.Patients_ResizeBegin);
            this.SizeChanged += new System.EventHandler(this.Patients_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion
        //private ListBoxBrowser listBrowser1;
        private CheckedListBrowser listBrowser1;
    }
}