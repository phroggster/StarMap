#region --- Apache v2.0 license ---
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
#endregion // --- Apache v2.0 license ---

#region --- using ... ---
using StarMap.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
#endregion // --- using ... ---

namespace StarMap
{
    public sealed class Config : INotifyPropertyChanged
    {
        #region --- private Config() ---

        private Config()
        {
            ResetToDefaults();
        }

        #endregion // --- private Config() ---

        #region --- public interfaces ---

        #region --- Configuration properties and associated events ---

        #region --- public Color CentredSystemColour { get; set; } ---

        private Color _CentredSystemColour = Color.Yellow;
        public event EventHandler<ColorEventArgs> CentredSystemColourChanged;
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

        #endregion // --- public Color CentredSystemColour { get; set; } ---

        #region --- public Color DefaultMapColour { get; set; } ---

        private Color _DefaultMapColour = Color.Red;
        public event EventHandler<ColorEventArgs> DefaultMapColourChanged;
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

        #endregion // --- public Color DefaultMapColour { get; set; } ---

        #region --- public Color FineGridLineColour { get; set; } ---

        //private Color _FineGridLinesColour = ColorTranslator.FromHtml("#202020");
        private Color _FineGridLineColour;
        public event EventHandler<ColorEventArgs> FineGridLineColourChanged;
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
        public static Color DefaultFineGridLineColour
        {
            get
            {
                return ColorTranslator.FromHtml("#BFBFBF");
            }
        }

        #endregion // --- public Color FineGridLineColour { get; set; } ---

        #region --- public Color GridLineColour { get; set; } ---

        private Color _GridLineColour;
        public event EventHandler<ColorEventArgs> GridLineColourChanged;
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
        public static Color DefaultGridLineColour
        {
            get
            {
                return ColorTranslator.FromHtml("#296A6C");
            }
        }

        #endregion // --- public Color GridLineColour { get; set; } ---

        #region --- public string HomeSystem { get; set; } ---

        private string _HomeSystem;
        public event EventHandler<StringEventArgs> HomeSystemChanged;
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
        public static string DefaultHomeSystem
        {
            get
            {
                return "Sol";
            }
        }

        #endregion // --- public string HomeSystem { get; set; } ---

        #region --- public int MouseSensitivity { get; set; } ---

        private int _MouseSensitivity;
        public event EventHandler<IntEventArgs> MouseSensitivityChanged;
        // Valid range: int.minvalue to int.maxvalue
        public int MouseSensitivity
        {
            get
            {
                return _MouseSensitivity;
            }
            set
            {
                SetInt(ref _MouseSensitivity, value, MouseSensitivityChanged);
            }
        }
        public static int DefaultMouseSensitivity
        {
            get
            {
                return 20000;
            }
        }

        #endregion // --- public float MouseSensitivity { get; set; } ---

        #region --- public bool VSync { get; set; } ---

        private bool _VSync;
        public event EventHandler<BoolEventArgs> VSyncChanged;
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
        public static bool DefaultVSync
        {
            get
            {
                return true;
            }
        }


        #endregion // --- public bool VSync { get; set; } ---

        // TODO: sorry/not sorry. Who wants to donate an Apple?
        public static string StarMapDB = @"%LOCALAPPDATA%\StarMap\settings.db";

        #endregion // --- Configuration properties and associated events ---

        #region --- INotifyPropertyChanged interface ---

        /// <summary>
        /// You should probably not use this event directly, instead see <see cref="ThreadedBindingList"/>,
        /// which is a class designed for safely binding Config properties directly to Control properties.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion // --- INotifyPropertyChanged interface ---

        #region --- Less exciting stuff ---

        public static bool IsLoaded { get; private set; } = false;

        public static Config Instance { get { return _Instance.Value; } }

        public void Load(params Dictionary<string, RegisterEntry>[] registers)
        {
            if (IsLoaded)
                return;
            if (registers == null)
                throw new ArgumentNullException(nameof(registers));

            IsLoaded = true;

            foreach (var r in registers)
            {
                if (r != null)
                {
                    // TODO: this needs to be moved to property attributes or something. Ugh.
                    if (r.ContainsKey("DefaultMap"))
                        _DefaultMapColour = r["DefaultMap"].ValueColor;

                    if (r.ContainsKey("DefaultMapCenter"))
                        _HomeSystem = r["DefaultMapCenter"].ValueString;

                    if (r.ContainsKey("MapColour_CentredSystem"))
                        _CentredSystemColour = r["MapColour_CentredSystem"].ValueColor;

                    if (r.ContainsKey("MapColour_CoarseGridLines"))
                        _GridLineColour = r["MapColour_CoarseGridLines"].ValueColor;

                    if (r.ContainsKey("MapColour_FineGridLines"))
                        _FineGridLineColour = r["MapColour_FineGridLines"].ValueColor;

                    if (r.ContainsKey(nameof(MouseSensitivity)))
                        _MouseSensitivity = r[nameof(MouseSensitivity)].ValueInt;

                    if (r.ContainsKey(nameof(VSync)))
                        _VSync = r[nameof(VSync)].ValueBool;
                }
            }
        }

        #endregion // --- Less exciting stuff ---

        #endregion // --- public interfaces ---

        #region --- private implementation ---

        private static Lazy<Config> _Instance = new Lazy<Config>(() => new Config());

        #region --- private bool Set<T>(ref T field, T newValue, EventHandler<T> event, string dbName) ---

        private bool SetBool(ref bool field, bool value, EventHandler<BoolEventArgs> handler,
            string EddbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field != value)
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EddbackendName))
                    SMDBConnection.PutSettingBool(propName, value);
                else
                    SMDBConnection.PutSettingBool(EddbackendName, value);
                OnPropertyChanged(propName, handler, new BoolEventArgs(value));
                return true;
            }
            return false;
        }

        private bool SetColor(ref Color field, Color value, EventHandler<ColorEventArgs> handler,
            string EDDbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field.ToArgb() != value.ToArgb())
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EDDbackendName))
                    SMDBConnection.PutSettingInt(propName, value.ToArgb());
                else
                    SMDBConnection.PutSettingInt(EDDbackendName, value.ToArgb());
                OnPropertyChanged(propName, handler, new ColorEventArgs(value));
                return true;
            }
            return false;
        }

        private bool SetDouble(ref double field, double value, EventHandler<DoubleEventArgs> handler,
            string EDDbackendName = null, [CallerMemberName] string propName = null)
        {
            // TODO: Implement this.
            throw new NotImplementedException();
        }

        private bool SetInt(ref int field, int value, EventHandler<IntEventArgs> handler,
            string EDDbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field != value)
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EDDbackendName))
                    SMDBConnection.PutSettingInt(propName, value);
                else
                    SMDBConnection.PutSettingInt(EDDbackendName, value);
                OnPropertyChanged(propName, handler, new IntEventArgs(value));
                return true;
            }
            return false;
        }

        private bool SetFloat(ref float field, float value, EventHandler<FloatEventArgs> handler,
            string EDDbackendName = null, [CallerMemberName] string propName = null)
        {
            if (field != value)
            {
                field = value;
                if (string.IsNullOrWhiteSpace(EDDbackendName))
                    SMDBConnection.PutSettingFloat(propName, value);
                else
                    SMDBConnection.PutSettingFloat(EDDbackendName, value);
                OnPropertyChanged(propName, handler, new FloatEventArgs(value));
                return true;
            }
            return false;
        }

        private bool SetString(ref string field, string value, EventHandler<StringEventArgs> handler,
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
                OnPropertyChanged(propName, handler, new StringEventArgs(value));
                return true;
            }
            return false;
        }

        #endregion // --- private Set<T>(ref T field, T newValue, EventHandler<T> event, string dbName) ---

        // Raises the following events:
        //         PropertyChanged
        //         <paramref> eventHandler
        private void OnPropertyChanged<T>(string propName, EventHandler<T> eventHandler, T newValue)
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

        private void ResetToDefaults()
        {
            _FineGridLineColour = DefaultFineGridLineColour;
            _GridLineColour = DefaultGridLineColour;
            _HomeSystem = DefaultHomeSystem;
            _MouseSensitivity = DefaultMouseSensitivity;
            _VSync = DefaultVSync;
        }

        #endregion // --- private implementation ---
    }
}
