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
            this.comboBoxSecond = new System.Windows.Forms.ComboBox();
            this.comboBoxMinute = new System.Windows.Forms.ComboBox();
            this.comboBoxHour = new System.Windows.Forms.ComboBox();
            this.comboBoxDay = new System.Windows.Forms.ComboBox();
            this.comboBoxMonth = new System.Windows.Forms.ComboBox();
            this.comboBoxYear = new System.Windows.Forms.ComboBox();
            this.labelSecond = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxSecond
            // 
            this.comboBoxSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSecond.FormattingEnabled = true;
            this.comboBoxSecond.Location = new System.Drawing.Point(88, 0);
            this.comboBoxSecond.Name = "comboBoxSecond";
            this.comboBoxSecond.Size = new System.Drawing.Size(50, 32);
            this.comboBoxSecond.TabIndex = 5;
            // 
            // comboBoxMinute
            // 
            this.comboBoxMinute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMinute.FormattingEnabled = true;
            this.comboBoxMinute.Location = new System.Drawing.Point(152, 0);
            this.comboBoxMinute.Name = "comboBoxMinute";
            this.comboBoxMinute.Size = new System.Drawing.Size(50, 32);
            this.comboBoxMinute.TabIndex = 4;
            // 
            // comboBoxHour
            // 
            this.comboBoxHour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxHour.FormattingEnabled = true;
            this.comboBoxHour.Location = new System.Drawing.Point(216, 0);
            this.comboBoxHour.Name = "comboBoxHour";
            this.comboBoxHour.Size = new System.Drawing.Size(50, 32);
            this.comboBoxHour.TabIndex = 3;
            // 
            // comboBoxDay
            // 
            this.comboBoxDay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDay.FormattingEnabled = true;
            this.comboBoxDay.Location = new System.Drawing.Point(285, 0);
            this.comboBoxDay.Name = "comboBoxDay";
            this.comboBoxDay.Size = new System.Drawing.Size(50, 32);
            this.comboBoxDay.TabIndex = 2;
            // 
            // comboBoxMonth
            // 
            this.comboBoxMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMonth.FormattingEnabled = true;
            this.comboBoxMonth.Location = new System.Drawing.Point(341, 0);
            this.comboBoxMonth.Name = "comboBoxMonth";
            this.comboBoxMonth.Size = new System.Drawing.Size(50, 32);
            this.comboBoxMonth.TabIndex = 1;
            // 
            // comboBoxYear
            // 
            this.comboBoxYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxYear.FormattingEnabled = true;
            this.comboBoxYear.Location = new System.Drawing.Point(397, 0);
            this.comboBoxYear.Name = "comboBoxYear";
            this.comboBoxYear.Size = new System.Drawing.Size(72, 32);
            this.comboBoxYear.TabIndex = 0;
            // 
            // labelSecond
            // 
            this.labelSecond.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSecond.Location = new System.Drawing.Point(139, 3);
            this.labelSecond.Name = "labelSecond";
            this.labelSecond.Size = new System.Drawing.Size(12, 28);
            this.labelSecond.TabIndex = 6;
            this.labelSecond.Text = ":";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(203, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 28);
            this.label2.TabIndex = 7;
            this.label2.Text = ":";
            // 
            // DateTimeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelSecond);
            this.Controls.Add(this.comboBoxYear);
            this.Controls.Add(this.comboBoxMonth);
            this.Controls.Add(this.comboBoxDay);
            this.Controls.Add(this.comboBoxHour);
            this.Controls.Add(this.comboBoxMinute);
            this.Controls.Add(this.comboBoxSecond);
            this.Name = "DateTimeEditor";
            this.Size = new System.Drawing.Size(470, 32);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ComboBox comboBoxSecond;
        public System.Windows.Forms.ComboBox comboBoxMinute;
        public System.Windows.Forms.ComboBox comboBoxHour;
        public System.Windows.Forms.ComboBox comboBoxDay;
        public System.Windows.Forms.ComboBox comboBoxMonth;
        public System.Windows.Forms.ComboBox comboBoxYear;
        public System.Windows.Forms.Label labelSecond;
        public System.Windows.Forms.Label label2;
    }
}
