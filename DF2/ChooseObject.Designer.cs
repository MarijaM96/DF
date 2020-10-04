namespace DF2
{
    partial class ChooseObject
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
            this.txtbox = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.rbtnAES = new System.Windows.Forms.RadioButton();
            this.rbtnPlain = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtbox
            // 
            this.txtbox.AllowDrop = true;
            this.txtbox.BackColor = System.Drawing.SystemColors.Menu;
            this.txtbox.Location = new System.Drawing.Point(22, 12);
            this.txtbox.Multiline = true;
            this.txtbox.Name = "txtbox";
            this.txtbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtbox.Size = new System.Drawing.Size(379, 158);
            this.txtbox.TabIndex = 1;
            this.txtbox.Text = "Insert text here...";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(326, 196);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
          //  this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // rbtnAES
            // 
            this.rbtnAES.AutoSize = true;
            this.rbtnAES.Location = new System.Drawing.Point(22, 196);
            this.rbtnAES.Name = "rbtnAES";
            this.rbtnAES.Size = new System.Drawing.Size(93, 17);
            this.rbtnAES.TabIndex = 3;
            this.rbtnAES.TabStop = true;
            this.rbtnAES.Text = "Hide with AES";
            this.rbtnAES.UseVisualStyleBackColor = true;
            // 
            // rbtnPlain
            // 
            this.rbtnPlain.AutoSize = true;
            this.rbtnPlain.Location = new System.Drawing.Point(129, 196);
            this.rbtnPlain.Name = "rbtnPlain";
            this.rbtnPlain.Size = new System.Drawing.Size(92, 17);
            this.rbtnPlain.TabIndex = 4;
            this.rbtnPlain.TabStop = true;
            this.rbtnPlain.Text = "Hide plain text";
            this.rbtnPlain.UseVisualStyleBackColor = true;
            // 
            // ChooseObject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 245);
            this.Controls.Add(this.rbtnPlain);
            this.Controls.Add(this.rbtnAES);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtbox);
            this.Name = "ChooseObject";
            this.Text = "Insert Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtbox;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.RadioButton rbtnAES;
        private System.Windows.Forms.RadioButton rbtnPlain;
    }
}