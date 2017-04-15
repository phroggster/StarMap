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

#region --- using ... ---
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
#endregion // --- using ... ---

namespace StarMap
{
    #region --- public class BoolEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="bool"/> property was changed.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class BoolEventArgs : EventArgs
    {
        private bool m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public bool Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="BoolEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="bool"/> property that is being communicated.</param>
        public BoolEventArgs(bool value)
        {
            m_Value = value;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(BoolEventArgs)}: {m_Value}";
            }
        }
    }

    #endregion // --- public class BoolEventArgs : EventArgs ---

    #region --- public class ColorEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="Color"/> property was changed.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ColorEventArgs : EventArgs
    {
        private Color m_Colour;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public Color Colour { get { return m_Colour; } }

        /// <summary>
        /// Constructs a new <see cref="ColorEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="Color"/> property that is being communicated.</param>
        public ColorEventArgs(Color colour)
        {
            m_Colour = colour;
        }

        private string DebuggerDisplay
        {
            get
            {
                if (m_Colour.IsNamedColor)
                    return m_Colour.Name;
                else if (m_Colour.IsKnownColor)
                    return m_Colour.ToKnownColor().ToString();
                else
                    return $"{nameof(ColorEventArgs)} RGBA: ({m_Colour.R}, {m_Colour.G}, {m_Colour.B}, {m_Colour.A})";
            }
        }
    }

    #endregion // --- public class ColorEventArgs : EventArgs ---

    #region --- public class DoubleEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="double"/> property was changed.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class DoubleEventArgs : EventArgs
    {
        private double m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public double Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="FloatEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="double"/> property that is being communicated.</param>
        public DoubleEventArgs(double value)
        {
            m_Value = value;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(DoubleEventArgs)}: {m_Value.ToString()}";
            }
        }
    }

    #endregion // --- public class DoubleEventArgs : EventArgs ---

    #region --- public class FloatEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="float"/> property was changed.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class FloatEventArgs : EventArgs
    {
        private float m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public float Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="FloatEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="float"/> property that is being communicated.</param>
        public FloatEventArgs(float value)
        {
            m_Value = value;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(FloatEventArgs)}: {m_Value.ToString()}";
            }
        }
    }

    #endregion // --- public class FloatEventArgs : EventArgs ---

    #region --- public class IntEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="int"/> property was changed.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class IntEventArgs : EventArgs
    {
        private int m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public int Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="IntEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="int"/> property that is being communicated.</param>
        public IntEventArgs(int value)
        {
            m_Value = value;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(IntEventArgs)}: {m_Value.ToString()}";
            }
        }
    }

    #endregion //--- public class IntEventArgs : EventArgs ---

    #region --- public class StringEventArgs : EventArgs ---

    /// <summary>
    /// An <see cref="EventArgs"/> class used to communicate that a <see cref="string"/> property was changed.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class StringEventArgs : EventArgs
    {
        private string m_Value;

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public string Value { get { return m_Value; } }

        /// <summary>
        /// Constructs a new <see cref="StringEventArgs"/> instance.
        /// </summary>
        /// <param name="value">The new value of the <see cref="string"/> property that is being communicated.</param>
        public StringEventArgs(string value)
        {
            m_Value = value;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{nameof(StringEventArgs)}: \"{m_Value}\"";
            }
        }
    }

    #endregion // --- public class StringEventArgs : EventArgs ---
}
