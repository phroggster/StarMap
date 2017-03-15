namespace StarMap
{
    partial class SettingsForm
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
            if (disposing)
            {
                _cbl?.Dispose();
                components?.Dispose();
            }
            _cbl = null;
            components = null;
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.coarseColor = new System.Windows.Forms.Panel();
            this.lblCourse = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fineColor = new System.Windows.Forms.Panel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.cbVSync = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.Location = new System.Drawing.Point(178, 166);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // coarseColor
            // 
            this.coarseColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(106)))), ((int)(((byte)(108)))));
            this.coarseColor.Location = new System.Drawing.Point(136, 9);
            this.coarseColor.Name = "coarseColor";
            this.coarseColor.Size = new System.Drawing.Size(24, 24);
            this.coarseColor.TabIndex = 4;
            this.coarseColor.Click += new System.EventHandler(this.colourPanel_Click);
            // 
            // lblCourse
            // 
            this.lblCourse.Location = new System.Drawing.Point(12, 9);
            this.lblCourse.Name = "lblCourse";
            this.lblCourse.Size = new System.Drawing.Size(118, 24);
            this.lblCourse.TabIndex = 5;
            this.lblCourse.Text = "Coarse Grid Lines:";
            this.lblCourse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "Fine Grid Lines:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fineColor
            // 
            this.fineColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.fineColor.Location = new System.Drawing.Point(136, 39);
            this.fineColor.Name = "fineColor";
            this.fineColor.Size = new System.Drawing.Size(24, 24);
            this.fineColor.TabIndex = 6;
            this.fineColor.Click += new System.EventHandler(this.colourPanel_Click);
            // 
            // cbVSync
            // 
            this.cbVSync.AutoSize = true;
            this.cbVSync.Location = new System.Drawing.Point(136, 69);
            this.cbVSync.Name = "cbVSync";
            this.cbVSync.Size = new System.Drawing.Size(57, 17);
            this.cbVSync.TabIndex = 8;
            this.cbVSync.Text = "VSync";
            this.cbVSync.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 201);
            this.Controls.Add(this.cbVSync);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fineColor);
            this.Controls.Add(this.lblCourse);
            this.Controls.Add(this.coarseColor);
            this.Controls.Add(this.btnOK);
            this.MinimumSize = new System.Drawing.Size(286, 116);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel coarseColor;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel fineColor;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox cbVSync;
    }
}