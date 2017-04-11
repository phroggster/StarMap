#region --- Apache v2.0 license ---
/*
 * Copyright © 2017 phroggie, StarMap development team
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StarMap
{
    #region --- public class BoolChangedEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="bool"/> property was changed.
    /// </summary>
    public class BoolChangedEventArgs : EventArgs
    {
        private bool m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public bool Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="BoolChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="bool"/> property that is being communicated.</param>
        public BoolChangedEventArgs(bool value)
        {
            m_Value = value;
        }
    }

    #endregion // --- public class BoolChangedEventArgs : EventArgs ---

    #region --- public class ColorChangedEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="Color"/> property was changed.
    /// </summary>
    public class ColorChangedEventArgs : EventArgs
    {
        private Color m_Colour;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public Color Colour { get { return m_Colour; } }

        /// <summary>
        /// Constructs a new <see cref="ColorChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="Color"/> property that is being communicated.</param>
        public ColorChangedEventArgs(Color colour)
        {
            m_Colour = colour;
        }
    }

    #endregion // --- public class ColorChangedEventArgs : EventArgs ---

    #region --- public class StringChangedEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="string"/> property was changed.
    /// </summary>
    public class StringChangedEventArgs : EventArgs
    {
        private string m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public string Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="StringChangedEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="string"/> property that is being communicated.</param>
        public StringChangedEventArgs(string value)
        {
            m_Value = value;
        }
    }

    #endregion // --- public class StringChangedEventArgs : EventArgs ---
}
