#region --- Apache v2.0 license ---
/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */
#endregion // --- Apache v2.0 license ---

#region --- using ... ---
using System;
using System.Text;
using System.Windows.Forms;
#endregion // --- using ... ---

namespace StarMap
{
    public partial class SettingsForm : Form
    {
        #region --- public SettingsForm() ---

        public SettingsForm()
        {
            InitializeComponent();
        }

        #endregion // --- public SettingsForm() ---

        #region --- private implementation ---

        private const int divizer = 10000;
        private ConfigBindingList _cbl;
        private decimal lastParse;

        #region --- Upstream event handlers ---

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _cbl = new ConfigBindingList();
            _cbl.BindToControl(cbVSync, nameof(cbVSync.Checked), nameof(Config.VSync));
            _cbl.BindToControl(pnlCoarseColor, nameof(pnlCoarseColor.BackColor), nameof(Config.GridLineColour));
            _cbl.BindToControl(pnlFineColor, nameof(pnlFineColor.BackColor), nameof(Config.FineGridLineColour));
            _cbl.BindToControl(trackBarSensitivity, nameof(trackBarSensitivity.Value), nameof(Config.MouseSensitivity));

            trackBarSensitivity_ValueChanged(this, EventArgs.Empty);
        }

        #endregion // --- Upstream event handlers ---

        #region --- Downstream event handlers ---

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            TraceLog.Notice($"{nameof(SettingsForm)}: Resetting configuration to defaults!");
            Config conf = Config.Instance;
            conf.FineGridLineColour = Config.DefaultFineGridLineColour;
            conf.GridLineColour = Config.DefaultGridLineColour;
            conf.MouseSensitivity = Config.DefaultMouseSensitivity;
            conf.VSync = Config.DefaultVSync;
        }

        private void colourPanel_Click(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            colorDialog1.Color = panel.BackColor;
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
                panel.BackColor = colorDialog1.Color;
        }

        private void trackBarSensitivity_ValueChanged(object sender, EventArgs e)
        {
            int val = trackBarSensitivity.Value;
            int msd = val / divizer;
            int lsd = Math.Abs(val - (msd * divizer));

            // TODO: Any way to handle this with globalization in mind? At least input is ok; only output sucks.
            textBoxSensitivity.Text = $"{msd}.{lsd:0000}";
        }

        private void textBoxSensitivity_Validated(object sender, EventArgs e)
        {
            int i = (int)(lastParse * divizer);
            Config.Instance.MouseSensitivity = i;
        }

        private void textBoxSensitivity_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxSensitivity.Text))
            {
                e.Cancel = true;
                return;
            }
            if (!decimal.TryParse(textBoxSensitivity.Text, out lastParse))
            {
                e.Cancel = true;
                MessageBox.Show("Unable to parse mouse sensitivity. Please try again.");
                return;
            }
            if (lastParse > 10 || lastParse < -10)
            {
                MessageBox.Show("Mouse sensitivity must be between negative 10 and positive 10.");
                e.Cancel = true;
                return;
            }
        }

        #endregion // --- Downstream event handlers ---

        #endregion // --- private implementation ---
    }
}
