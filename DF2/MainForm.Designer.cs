namespace DF2
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.uploadCoverImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideWithToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.revealHiddenTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pbxcover = new System.Windows.Forms.PictureBox();
            this.pbxstego = new System.Windows.Forms.PictureBox();
            this.saveToolStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.loadformatToolstrip = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxcover)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxstego)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uploadCoverImageToolStripMenuItem,
            this.hideWithToolStripMenuItem,
            this.revealHiddenTextToolStripMenuItem,
            this.saveToolStrip,
            this.loadformatToolstrip});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(661, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // uploadCoverImageToolStripMenuItem
            // 
            this.uploadCoverImageToolStripMenuItem.Name = "uploadCoverImageToolStripMenuItem";
            this.uploadCoverImageToolStripMenuItem.Size = new System.Drawing.Size(125, 20);
            this.uploadCoverImageToolStripMenuItem.Text = "Upload cover image";
            this.uploadCoverImageToolStripMenuItem.Click += new System.EventHandler(this.uploadCoverImageToolStripMenuItem_Click);
            // 
            // hideWithToolStripMenuItem
            // 
            this.hideWithToolStripMenuItem.Name = "hideWithToolStripMenuItem";
            this.hideWithToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.hideWithToolStripMenuItem.Text = "Hide text";
            this.hideWithToolStripMenuItem.Click += new System.EventHandler(this.hideWithToolStripMenuItem_Click);
            // 
            // revealHiddenTextToolStripMenuItem
            // 
            this.revealHiddenTextToolStripMenuItem.Name = "revealHiddenTextToolStripMenuItem";
            this.revealHiddenTextToolStripMenuItem.Size = new System.Drawing.Size(115, 20);
            this.revealHiddenTextToolStripMenuItem.Text = "Reveal hidden text";
            this.revealHiddenTextToolStripMenuItem.Click += new System.EventHandler(this.revealHiddenTextToolStripMenuItem_Click);
            // 
            // pbxcover
            // 
            this.pbxcover.Location = new System.Drawing.Point(12, 27);
            this.pbxcover.Name = "pbxcover";
            this.pbxcover.Size = new System.Drawing.Size(308, 380);
            this.pbxcover.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxcover.TabIndex = 1;
            this.pbxcover.TabStop = false;
            // 
            // pbxstego
            // 
            this.pbxstego.Location = new System.Drawing.Point(342, 27);
            this.pbxstego.Name = "pbxstego";
            this.pbxstego.Size = new System.Drawing.Size(308, 380);
            this.pbxstego.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxstego.TabIndex = 2;
            this.pbxstego.TabStop = false;
            // 
            // saveToolStrip
            // 
            this.saveToolStrip.Name = "saveToolStrip";
            this.saveToolStrip.Size = new System.Drawing.Size(125, 20);
            this.saveToolStrip.Text = "Save as New Format";
            this.saveToolStrip.Visible = false;
            this.saveToolStrip.Click += new System.EventHandler(this.saveToolStrip_Click);
            // 
            // loadformatToolstrip
            // 
            this.loadformatToolstrip.Name = "loadformatToolstrip";
            this.loadformatToolstrip.Size = new System.Drawing.Size(117, 20);
            this.loadformatToolstrip.Text = "Load From Format";
            this.loadformatToolstrip.Visible = false;
            this.loadformatToolstrip.Click += new System.EventHandler(this.loadformatToolstrip_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 413);
            this.Controls.Add(this.pbxstego);
            this.Controls.Add(this.pbxcover);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Steganography";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxcover)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxstego)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem uploadCoverImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideWithToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem revealHiddenTextToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbxcover;
        private System.Windows.Forms.PictureBox pbxstego;
        private System.Windows.Forms.ToolStripMenuItem saveToolStrip;
        private System.Windows.Forms.ToolStripMenuItem loadformatToolstrip;
    }
}

