#region --- Apache v2.0 license ---
/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 * Copyright © 2017 EDDiscovery development team
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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
#endregion // --- using ... ---

namespace StarMap
{
    /// <summary>
    /// A context-aware <see cref="BindingList{T}"/> (type <see cref="Config"/>) wrapper to easily bind
    /// <see cref="Component"/> properties to <see cref="Config"/> properties.
    /// </summary>
    public class ConfigBindingList : BindingList<Config>, IIsDisposed
    {
        #region --- public ConfigBindingList() ---

        /// <summary>
        /// Constructs a new <see cref="ConfigBindingList"/> instance.
        /// </summary>
        public ConfigBindingList()
        {
            _ctx = SynchronizationContext.Current;
            _bs = new BindingSource();
            _bs.Add(this);
            Add(Config.Instance);
        }

        #endregion // --- public ConfigBindingList() ---

        #region --- public interfaces ---

        #region --- public void BindToControl(IBindableComponent, string, string, DataSourceUpdateMode) ---

        /// <summary>
        /// Convenience method to bind a <see cref="Control"/> property to a <see cref="Config"/> property without data formatting.
        /// </summary>
        /// <param name="control">The <see cref="IBindableComponent"/> that shall be bound to the <paramref name="configProp"/>.</param>
        /// <param name="controlProp">The control's property that shall be bound to, such as <c>Text</c>, <c>Checked</c>, etc.</param>
        /// <param name="configProp">A named <see cref="Config"/> property to bind to the control.</param>
        /// <param name="mode">The update mode to use. For a <see cref="TextBox"/> control, or something else with
        /// validation logic, set this to <see cref="DataSourceUpdateMode.OnValidation"/>. Simple control bindings
        /// (such as a <see cref="CheckBox"/>, <see cref="RadioButton"/>, etc), the default value is ideal.</param>
        /// <example><code>private ConfigBindingList cbl;
        /// 
        /// private void Form1_Load(object sender, EventArgs e) {
        ///     cbl = new ConfigBindingList();
        ///     cbl.BindToControl(textBox1, nameof(textBox1.Text), nameof(Config.HomeSystem), DataSourceUpdateMode.OnValidation);
        /// }
        /// 
        /// protected void Dispose(bool disposing) {
        ///     if (disposing) {
        ///         cbl?.Dispose();
        ///         components?.Dispose();
        ///     }
        ///     base.Dispose(disposing);
        /// }</code></example>
        public void BindToControl(IBindableComponent control, string controlProp, string configProp, DataSourceUpdateMode mode = DataSourceUpdateMode.OnPropertyChanged)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ConfigBindingList));
            else if (_ctx != null && !_ctx.Equals(SynchronizationContext.Current))
                throw new InvalidOperationException("SynchronizationContext is invalid.");

            control.DataBindings.Add(controlProp, this, configProp, false, mode);
        }

        #endregion // --- public void BindToControl(IBindableComponent, string, string, DataSourceUpdateMode) ---

        #region --- IIsDisposed ---

        /// <summary>
        /// Whether or not this <see cref="ConfigBindingList"/> is disposed.
        /// </summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// Releases all resources consumed by this <see cref="ConfigBindingList"/> instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion // --- IIsDisposed ---

        #endregion // --- public interfaces ---

        #region --- protected/private implementation ---

        /// <summary>
        /// Releases the unmanaged resources used by this <see cref="ConfigBindingList"/> instance,
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                if (disposing)
                {
                    Clear();
                    if (_bs != null)
                    {
                        _bs.Clear();
                        _bs.Dispose();
                    }
                }
                else
                    TraceLog.Warn($"{nameof(ConfigBindingList)} leaked! Did you forget to call `Dispose()`?");
                _bs = null;
                _ctx = null;
            }
        }

        /// <summary>
        /// Raises the <see cref="BindingList{T}.ListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ListChangedEventArgs"/> that contains the event data.</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (!IsDisposed)
            {
                if (_ctx == null)
                    base.OnListChanged(e);
                else
                    _ctx.Send(delegate
                    {
                        base.OnListChanged(e);
                    }, null);
            }
        }

        private BindingSource _bs;
        private SynchronizationContext _ctx;

        ~ConfigBindingList()
        {
            Dispose(false);
        }

        #endregion // --- protected/private implementation ---
    }
}
