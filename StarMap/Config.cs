/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 * Copyright © 2015 - 2017 EDDiscovery development team
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
using StarMap.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace StarMap
{
    public sealed class Config : INotifyPropertyChanged
    {
        #region Props, backers, and prop changed events

        private Color _CentredSystemColour = Color.Yellow;
        public event EventHandler<Color> CentredSystemColourChanged;
        public Color CentredSystemColour
        {
            get
            {
                return _CentredSystemColour;
            }
            set
            {
                SetColor(ref _CentredSystemColour, value, CentredSystemColourChanged, "MapColour_CentredSystem");
            }
        }

        private Color _DefaultMapColour = Color.Red;
        public event EventHandler<Color> DefaultMapColourChanged;
        public Color DefaultMapColour
        {
            get
            {
                return _DefaultMapColour;
            }
            set
            {
                SetColor(ref _DefaultMapColour, value, DefaultMapColourChanged, "DefaultMap");
            }
        }

        //private Color _FineGridLinesColour = ColorTranslator.FromHtml("#202020");
        private Color _FineGridLineColour = ColorTranslator.FromHtml("#BFBFBF");
        public event EventHandler<Color> FineGridLineColourChanged;
        public Color FineGridLineColour
        {
            get
            {
                return _FineGridLineColour;
            }
            set
            {
                SetColor(ref _FineGridLineColour, value, FineGridLineColourChanged, "MapColour_FineGridLines");
            }
        }

        private Color _GridLineColour = ColorTranslator.FromHtml("#296A6C");
        public event EventHandler<Color> GridLineColourChanged;
        public Color GridLineColour
        {
            get
            {
                return _GridLineColour;
            }
            set
            {
                SetColor(ref _GridLineColour, value, GridLineColourChanged, "MapColour_CoarseGridLines");
            }
        }

        private string _HomeSystem = "Sol";
        public event EventHandler<string> HomeSystemChanged;
        public string HomeSystem
        {
            get
            {
                return _HomeSystem;
            }
            set
            {
                SetString(ref _HomeSystem, value, HomeSystemChanged, "DefaultMapCenter");
            }
        }

        private bool _VSync = true;
        public event EventHandler<bool> VSyncChanged;
        public bool VSync
        {
            get
            {
                return _VSync;
            }
            set
            {
                SetBool(ref _VSync, value, VSyncChanged);
            }
        }

        // sorry/not sorry
        public static string StarMapDB = @"%LOCALAPPDATA%\StarMap\settings.db";

        #endregion // Props, backers, and prop changed events

        #region Less exciting stuff

        public static bool IsLoaded { get; private set; } = false;

        public static readonly Lazy<Config> _Instance = new Lazy<Config>(() => new Config());
        public static Config Instance { get { return _Instance.Value; } }

        /// <summary>
        /// You should probably not use this event directly, instead see <see cref="ThreadedBindingList"/>,
        /// which is a class designed for safely binding Config properties directly to Control properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void Load()
        {
            // don't let multiple threads run Load() when the static instance is already in a valid state (or will be very shortly).
            if (IsLoaded)
                return;
            IsLoaded = true;

            // Since our DB may not exist, start off with the user's EDD configuration
            foreach (var r in new Dictionary<string, RegisterEntry>[]{ EDDUserDBConnection.EarlyRegister, SMDBConnection.EarlyRegister }) {
                if (r != null)
                {
                    if (r.ContainsKey("MapColour_CentredSystem"))
                        _CentredSystemColour = r["MapColour_CentredSystem"].ValueColor;

                    if (r.ContainsKey("DefaultMap"))
                        _DefaultMapColour = r["DefaultMap"].ValueColor;

                    if (r.ContainsKey("MapColour_FineGridLines"))
                        _FineGridLineColour = r["MapColour_FineGridLines"].ValueColor;

                    if (r.ContainsKey("MapColour_CoarseGridLines"))
                        _GridLineColour = r["MapColour_CoarseGridLines"].ValueColor;

                    if (r.ContainsKey("MapColour_CentredSystem"))
                        _CentredSystemColour = r["MapColour_CentredSystem"].ValueColor;

                    if (r.ContainsKey("DefaultMapCenter"))
                        _HomeSystem = r["DefaultMapCenter"].ValueString;

                    if (r.ContainsKey("VSync"))
                        _VSync = r["VSync"].ValueBool;
                }
            }
        }

        public void OnPropertyChanged<T>(string propName, EventHandler<T> eventHandler, T newValue)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

            if (eventHandler != null)
            {
                foreach (EventHandler<T> hdlr in eventHandler.GetInvocationList())
                {
                    var sync = hdlr.Target as ISynchronizeInvoke;
                    if (sync != null && sync.InvokeRequired)
                        sync.Invoke(hdlr, new object[] { this, newValue });
                    else
                        hdlr.Invoke(this, newValue);
                }
            }
        }

        private bool SetBool(ref bool field, bool value, EventHandler<bool> handler,
            string EddbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field != value)
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EddbackendName))
                    SMDBConnection.PutSettingBool(propName, value);
                else
                    SMDBConnection.PutSettingBool(EddbackendName, value);
                OnPropertyChanged(propName, handler, value);
                return true;
            }
            return false;
        }

        private bool SetColor(ref Color field, Color value, EventHandler<Color> handler,
            string EDDbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field.ToArgb() != value.ToArgb())
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EDDbackendName))
                    SMDBConnection.PutSettingInt(propName, value.ToArgb());
                else
                    SMDBConnection.PutSettingInt(EDDbackendName, value.ToArgb());
                OnPropertyChanged(propName, handler, value);
                return true;
            }
            return false;
        }

        private bool SetString(ref string field, string value, EventHandler<string> handler,
            string EDDbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field == null && value == null)
                return false;

            if ((field != null && !field.Equals(value)) || (value != null && !value.Equals(field)))
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EDDbackendName))
                    SMDBConnection.PutSettingString(propName, value);
                else
                    SMDBConnection.PutSettingString(EDDbackendName, value);
                OnPropertyChanged(propName, handler, value);
                return true;
            }
            return false;
        }

        #endregion // Less exciting stuff
    }
}
