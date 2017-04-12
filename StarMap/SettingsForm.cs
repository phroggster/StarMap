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
using System.Windows.Forms;
#endregion // --- using ... ---

namespace StarMap
{
    public partial class SettingsForm : Form
    {
        private ConfigBindingList _cbl;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            _cbl = new ConfigBindingList();
            _cbl.BindToControl(cbVSync, nameof(cbVSync.Checked), nameof(Config.VSync));
            _cbl.BindToControl(pnlCoarseColor, nameof(pnlCoarseColor.BackColor), nameof(Config.GridLineColour));
            _cbl.BindToControl(pnlFineColor, nameof(pnlFineColor.BackColor), nameof(Config.FineGridLineColour));
        }

        #region Downstream event handlers

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void colourPanel_Click(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            colorDialog1.Color = panel.BackColor;
            if (colorDialog1.ShowDialog(this) == DialogResult.OK)
                panel.BackColor = colorDialog1.Color;
        }

        #endregion // Downstream event handlers
    }
}
