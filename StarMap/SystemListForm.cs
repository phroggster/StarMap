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
using StarMap.Database;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
#endregion // --- using ... ---

namespace StarMap
{
    public partial class SystemListForm : Form
    {
        private BindingList<SystemBase> _bl;

        public SystemListForm()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            if (SystemsManager.IsLoaded)
                DataIsReady();
            else if (SystemsManager.IsLoading)
                backgroundWorker1.RunWorkerAsync();
        }

        private void DataIsReady()
        {
            _bl = new BindingList<SystemBase>(SystemsManager.SystemsList);
            dataGridView1.DataSource = _bl;
            dataGridView1.Visible = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var bgw = sender as BackgroundWorker;
            while (SystemsManager.IsLoading && !bgw.CancellationPending)
                Thread.Sleep(750); // Load is in progress, so let's just take a nap.
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!IsDisposed && e != null && !e.Cancelled && SystemsManager.SystemsList != null)
                DataIsReady();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (lblLoading.Visible)
                lblLoading.Visible = false;
        }

        private void dataGridView1_BindingContextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
