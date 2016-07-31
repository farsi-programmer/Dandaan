namespace Dandaan.UserControls
{
    partial class DateTimeEditor
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
            this.labelSecond = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSecond = new System.Windows.Forms.TextBox();
            this.textBoxMinute = new System.Windows.Forms.TextBox();
            this.textBoxYear = new System.Windows.Forms.TextBox();
            this.textBoxMonth = new System.Windows.Forms.TextBox();
            this.textBoxDay = new System.Windows.Forms.TextBox();
            this.labelDay = new System.Windows.Forms.Label();
            this.textBoxHour = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelSecond
            // 
            this.labelSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSecond.Location = new System.Drawing.Point(139, 0);
            this.labelSecond.Name = "labelSecond";
            this.labelSecond.Size = new System.Drawing.Size(12, 28);
            this.labelSecond.TabIndex = 5;
            this.labelSecond.Text = ":";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(75, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 28);
            this.label2.TabIndex = 7;
            this.label2.Text = ":";
            // 
            // textBoxSecond
            // 
            this.textBoxSecond.Location = new System.Drawing.Point(24, 0);
            this.textBoxSecond.Name = "textBoxSecond";
            this.textBoxSecond.Size = new System.Drawing.Size(50, 29);
            this.textBoxSecond.TabIndex = 8;
            // 
            // textBoxMinute
            // 
            this.textBoxMinute.Location = new System.Drawing.Point(88, 0);
            this.textBoxMinute.Name = "textBoxMinute";
            this.textBoxMinute.Size = new System.Drawing.Size(50, 29);
            this.textBoxMinute.TabIndex = 6;
            // 
            // textBoxYear
            // 
            this.textBoxYear.Location = new System.Drawing.Point(397, 0);
            this.textBoxYear.Name = "textBoxYear";
            this.textBoxYear.Size = new System.Drawing.Size(73, 29);
            this.textBoxYear.TabIndex = 0;
            // 
            // textBoxMonth
            // 
            this.textBoxMonth.Location = new System.Drawing.Point(341, 0);
            this.textBoxMonth.Name = "textBoxMonth";
            this.textBoxMonth.Size = new System.Drawing.Size(50, 29);
            this.textBoxMonth.TabIndex = 1;
            // 
            // textBoxDay
            // 
            this.textBoxDay.Location = new System.Drawing.Point(285, 0);
            this.textBoxDay.Name = "textBoxDay";
            this.textBoxDay.Size = new System.Drawing.Size(50, 29);
            this.textBoxDay.TabIndex = 2;
            // 
            // labelDay
            // 
            this.labelDay.AutoSize = true;
            this.labelDay.Location = new System.Drawing.Point(246, 3);
            this.labelDay.Name = "labelDay";
            this.labelDay.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.labelDay.Size = new System.Drawing.Size(37, 24);
            this.labelDay.TabIndex = 3;
            this.labelDay.Text = "شنبه";
            // 
            // textBoxHour
            // 
            this.textBoxHour.Location = new System.Drawing.Point(152, 0);
            this.textBoxHour.Name = "textBoxHour";
            this.textBoxHour.Size = new System.Drawing.Size(50, 29);
            this.textBoxHour.TabIndex = 4;
            // 
            // DateTimeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxHour);
            this.Controls.Add(this.labelDay);
            this.Controls.Add(this.textBoxDay);
            this.Controls.Add(this.textBoxMonth);
            this.Controls.Add(this.textBoxYear);
            this.Controls.Add(this.textBoxMinute);
            this.Controls.Add(this.textBoxSecond);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelSecond);
            this.Name = "DateTimeEditor";
            this.Size = new System.Drawing.Size(470, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.Label labelSecond;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox textBoxSecond;
        public System.Windows.Forms.TextBox textBoxMinute;
        public System.Windows.Forms.TextBox textBoxYear;
        public System.Windows.Forms.TextBox textBoxMonth;
        public System.Windows.Forms.TextBox textBoxDay;
        public System.Windows.Forms.Label labelDay;
        public System.Windows.Forms.TextBox textBoxHour;
    }
}
