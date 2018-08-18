namespace EdiZonDebugger
{
    partial class SaveSelector
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
            this.saveComboBox = new System.Windows.Forms.ComboBox();
            this.OKBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // saveComboBox
            // 
            this.saveComboBox.FormattingEnabled = true;
            this.saveComboBox.Location = new System.Drawing.Point(12, 25);
            this.saveComboBox.Name = "saveComboBox";
            this.saveComboBox.Size = new System.Drawing.Size(226, 21);
            this.saveComboBox.TabIndex = 0;
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(12, 52);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(81, 23);
            this.OKBtn.TabIndex = 1;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select a save file:";
            // 
            // SaveSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(250, 86);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.saveComboBox);
            this.Name = "SaveSelector";
            this.Text = "Save Selector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox saveComboBox;
        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.Label label1;
    }
}