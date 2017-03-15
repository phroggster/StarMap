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
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace StarMap
{
    /// <summary>
    /// A thread-safe BindingList{Config} wrapper to easily bind control properties to <see cref="Config"/> properties.
    /// </summary>
    public class ConfigBindingList : BindingList<Config>, IDisposable
    {
        public bool IsDisposed { get; private set; } = false;
        private BindingSource _bs;
        private SynchronizationContext _ctx;

        public ConfigBindingList()
        {
            _ctx = SynchronizationContext.Current;
            _bs = new BindingSource();
            _bs.Add(this);
            Add(Config.Instance);
        }

        ~ConfigBindingList()
        {
#if DEBUG
            Debug.Print("[WARN] ConfigBindingList leaked. Did you forget to call Dispose()?");
#endif
            Dispose(false);
        }

        /// <summary>
        /// Convenience method to bind a <see cref="Control"/> property to an <see cref="EDDConfig"/> property
        /// with immediate updates for controls such as a checkbox or radio buttons.
        /// </summary>
        /// <param name="control">The control that shall be bound to the <paramref name="configProp"/>.</param>
        /// <param name="controlProp">The control's property that shall be bound to, such as Text, Checked, etc.</param>
        /// <param name="configProp">A named <see cref="Config"/> property to bind to the control.</param>
        public void Bind(IBindableComponent control, string controlProp, string configProp)
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().FullName);
            control.DataBindings.Add(controlProp, this, configProp, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        /// <summary>
        /// Convenience method to bind a <see cref="Control"/> property to an <see cref="EDDConfig"/> property
        /// with updates occuring after the control has validated any input.
        /// </summary>
        /// <param name="control">The control that shall be bound to the <paramref name="configProp"/>.</param>
        /// <param name="controlProp">The control's property that shall be bound to, such as Text, Checked, etc.</param>
        /// <param name="configProp">A named <see cref="EDDConfig"/> property to bind to the control.</param>
        public void BindValidated(IBindableComponent control, string controlProp, string configProp)
        {
            if (IsDisposed) throw new ObjectDisposedException(GetType().FullName);
            control.DataBindings.Add(controlProp, this, configProp, false, DataSourceUpdateMode.OnValidation);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Clear();
                    _bs?.Clear();
                }
                _bs = null;
                _ctx = null;
                IsDisposed = true;
            }
        }

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
    }
}
