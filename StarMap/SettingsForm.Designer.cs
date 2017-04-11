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
                if (_cbl != null)
                    _cbl.Dispose();
                if (components != null)
                    components.Dispose();
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
            this.lblCoarse = new System.Windows.Forms.Label();
            this.lblFine = new System.Windows.Forms.Label();
            this.pnlCoarseColor = new System.Windows.Forms.Panel();
            this.pnlFineColor = new System.Windows.Forms.Panel();
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
            // lblCoarse
            // 
            this.lblCoarse.Location = new System.Drawing.Point(12, 9);
            this.lblCoarse.Name = "lblCoarse";
            this.lblCoarse.Size = new System.Drawing.Size(118, 24);
            this.lblCoarse.TabIndex = 5;
            this.lblCoarse.Text = "Coarse Grid Lines:";
            this.lblCoarse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblFine
            // 
            this.lblFine.Location = new System.Drawing.Point(12, 39);
            this.lblFine.Name = "lblFine";
            this.lblFine.Size = new System.Drawing.Size(118, 24);
            this.lblFine.TabIndex = 7;
            this.lblFine.Text = "Fine Grid Lines:";
            this.lblFine.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlCoarseColor
            // 
            this.pnlCoarseColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(106)))), ((int)(((byte)(108)))));
            this.pnlCoarseColor.Location = new System.Drawing.Point(136, 9);
            this.pnlCoarseColor.Name = "pnlCoarseColor";
            this.pnlCoarseColor.Size = new System.Drawing.Size(24, 24);
            this.pnlCoarseColor.TabIndex = 4;
            this.pnlCoarseColor.Click += new System.EventHandler(this.colourPanel_Click);
            // 
            // pnlFineColor
            // 
            this.pnlFineColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.pnlFineColor.Location = new System.Drawing.Point(136, 39);
            this.pnlFineColor.Name = "pnlFineColor";
            this.pnlFineColor.Size = new System.Drawing.Size(24, 24);
            this.pnlFineColor.TabIndex = 6;
            this.pnlFineColor.Click += new System.EventHandler(this.colourPanel_Click);
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
            this.Controls.Add(this.lblFine);
            this.Controls.Add(this.pnlFineColor);
            this.Controls.Add(this.lblCoarse);
            this.Controls.Add(this.pnlCoarseColor);
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
        private System.Windows.Forms.Label lblCoarse;
        private System.Windows.Forms.Label lblFine;
        private System.Windows.Forms.Panel pnlCoarseColor;
        private System.Windows.Forms.Panel pnlFineColor;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox cbVSync;
    }
}