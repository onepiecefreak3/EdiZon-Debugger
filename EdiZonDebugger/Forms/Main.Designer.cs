namespace EdiZonDebugger
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractEditedSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoriesListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.errorTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.extractEditedSaveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openToolStripMenuItem.Text = "&Open Config";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.closeToolStripMenuItem.Text = "&Close Config";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // extractEditedSaveToolStripMenuItem
            // 
            this.extractEditedSaveToolStripMenuItem.Name = "extractEditedSaveToolStripMenuItem";
            this.extractEditedSaveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.extractEditedSaveToolStripMenuItem.Text = "&Extract Edited Save";
            this.extractEditedSaveToolStripMenuItem.Click += new System.EventHandler(this.extractEditedSaveToolStripMenuItem_Click);
            // 
            // categoriesListBox
            // 
            this.categoriesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.categoriesListBox.FormattingEnabled = true;
            this.categoriesListBox.Location = new System.Drawing.Point(12, 27);
            this.categoriesListBox.Name = "categoriesListBox";
            this.categoriesListBox.Size = new System.Drawing.Size(120, 264);
            this.categoriesListBox.TabIndex = 1;
            this.categoriesListBox.SelectedIndexChanged += new System.EventHandler(this.categoriesListBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Location = new System.Drawing.Point(138, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 264);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // errorTextBox
            // 
            this.errorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorTextBox.BackColor = System.Drawing.SystemColors.Desktop;
            this.errorTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.errorTextBox.ForeColor = System.Drawing.SystemColors.Window;
            this.errorTextBox.Location = new System.Drawing.Point(462, 43);
            this.errorTextBox.Name = "errorTextBox";
            this.errorTextBox.ReadOnly = true;
            this.errorTextBox.Size = new System.Drawing.Size(326, 248);
            this.errorTextBox.TabIndex = 0;
            this.errorTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(462, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Console:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 300);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.errorTextBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.categoriesListBox);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "EdiZon Debugger";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ListBox categoriesListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RichTextBox errorTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem extractEditedSaveToolStripMenuItem;
    }
}

