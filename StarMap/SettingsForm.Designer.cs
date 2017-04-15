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
            this.lblMouseSensitivity = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.trackBarSensitivity = new System.Windows.Forms.TrackBar();
            this.textBoxSensitivity = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(156, 135);
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
            this.cbVSync.Checked = true;
            this.cbVSync.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVSync.Location = new System.Drawing.Point(174, 12);
            this.cbVSync.Name = "cbVSync";
            this.cbVSync.Size = new System.Drawing.Size(57, 17);
            this.cbVSync.TabIndex = 8;
            this.cbVSync.Text = "&VSync";
            this.cbVSync.UseVisualStyleBackColor = true;
            // 
            // lblMouseSensitivity
            // 
            this.lblMouseSensitivity.AutoSize = true;
            this.lblMouseSensitivity.Location = new System.Drawing.Point(38, 72);
            this.lblMouseSensitivity.Name = "lblMouseSensitivity";
            this.lblMouseSensitivity.Size = new System.Drawing.Size(92, 13);
            this.lblMouseSensitivity.TabIndex = 10;
            this.lblMouseSensitivity.Text = "Mouse Sensitivity:";
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.Location = new System.Drawing.Point(12, 135);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 11;
            this.btnReset.Text = "&Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // trackBarSensitivity
            // 
            this.trackBarSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarSensitivity.LargeChange = 10000;
            this.trackBarSensitivity.Location = new System.Drawing.Point(12, 89);
            this.trackBarSensitivity.Maximum = 100000;
            this.trackBarSensitivity.Minimum = -100000;
            this.trackBarSensitivity.Name = "trackBarSensitivity";
            this.trackBarSensitivity.Size = new System.Drawing.Size(219, 45);
            this.trackBarSensitivity.SmallChange = 5000;
            this.trackBarSensitivity.TabIndex = 14;
            this.trackBarSensitivity.TickFrequency = 10000;
            this.trackBarSensitivity.Value = 20000;
            this.trackBarSensitivity.ValueChanged += new System.EventHandler(this.trackBarSensitivity_ValueChanged);
            // 
            // textBoxSensitivity
            // 
            this.textBoxSensitivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSensitivity.Location = new System.Drawing.Point(136, 69);
            this.textBoxSensitivity.Name = "textBoxSensitivity";
            this.textBoxSensitivity.Size = new System.Drawing.Size(95, 20);
            this.textBoxSensitivity.TabIndex = 15;
            this.textBoxSensitivity.Text = "2.0";
            this.textBoxSensitivity.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxSensitivity_Validating);
            this.textBoxSensitivity.Validated += new System.EventHandler(this.textBoxSensitivity_Validated);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 170);
            this.Controls.Add(this.textBoxSensitivity);
            this.Controls.Add(this.trackBarSensitivity);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lblMouseSensitivity);
            this.Controls.Add(this.cbVSync);
            this.Controls.Add(this.lblFine);
            this.Controls.Add(this.pnlFineColor);
            this.Controls.Add(this.lblCoarse);
            this.Controls.Add(this.pnlCoarseColor);
            this.MinimumSize = new System.Drawing.Size(259, 209);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).EndInit();
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
        private System.Windows.Forms.Label lblMouseSensitivity;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TrackBar trackBarSensitivity;
        private System.Windows.Forms.TextBox textBoxSensitivity;
    }
}