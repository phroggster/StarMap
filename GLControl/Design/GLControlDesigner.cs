#region License
//
// The Open Toolkit Library License
//
// Author:
//       phroggie <phroggster@gmail.com>
//
// Copyright (c) 2017 phroggie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Phroggiesoft.Controls.Design
{
    // <summary>
    // The designer "brains" behind the <see cref="GLControl"/>.
    // </summary>
    // <seealso cref="GLControl"/>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    internal class GLControlDesigner : ControlDesigner
    {
        #region --- Constructor ---

        // <summary>
        // Constructs a new <see cref="GLControlDesigner"/> instance.
        // </summary>
        public GLControlDesigner()
        {
            if (OpenGLLogo == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Resources.OpenGL_Logo.png";
                using (Stream stream = assembly.GetManifestResourceStream(typeof(GLControl), resourceName))
                    OpenGLLogo = Image.FromStream(stream);
            }
        }

        #endregion // --- Constructor ---

        #region --- Protected virtual string[] properties to allow for filtering ---

        // <summary>
        // Events that will be filtered out and not displayed for controls assigned to this <see cref="GLControlDesigner"/>.
        // </summary>
        protected virtual string[] FilteredEvents { get { return _FilteredEvents; } }

        // <summary>
        // Properties that will be filtered out and not displayed for controls assigned to this <see cref="GLControlDesigner"/>.
        // </summary>
        protected virtual string[] FilteredProperties { get { return _FilteredProperties; } }

        #endregion // --- Protected virtual string[] properties to allow for filtering ---

        #region --- public override void Initialize(IComponent component) ---

        // <summary>
        // Initializes the designer with the specified component.
        // </summary>
        // <param name="component">The <see cref="IComponent"/> to associate the designer with. This component
        // must always be an instance of, or derive from, <see cref="Control"/>.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            IComponentChangeService svc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (svc != null)
            {
                svc.ComponentChanged -= ComponentChangeService_ComponentChanged;
                svc.ComponentChanged += ComponentChangeService_ComponentChanged;
            }
        }

        #endregion --- public override void Initialize(IComponent component) ---

        #region --- protected override void Dispose(bool disposing) ---

        // <summary>
        // Releases the unmanaged resources used by the <see cref="GLControlDesigner"/> and
        // optionally releases the managed resources.
        // </summary>
        // <param name="disposing"><c>true</c> to release both managed and unmanaged resources;
        // <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IComponentChangeService svc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                if (svc != null)
                    svc.ComponentChanged -= ComponentChangeService_ComponentChanged;
                OpenGLLogo?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion // --- protected override void Dispose(bool disposing) ---

        #region --- protected override void OnPaintAdornments(PaintEventArgs pe) ---

        // <summary>
        // Receives a call when the control that the designer is managing has painted its surface
        // so the designer can paint any additional adornments on top of the control.
        // </summary>
        // <param name="pe">A <see cref="PaintEventArgs"/> the designer can use to draw on the control.</param>
        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            if (Control != null)
            {
                // Why /not/ just do the painting here? Seems ideal...
                using (Brush b = new SolidBrush(SystemColors.ControlText))
                    pe.Graphics.FillRectangle(b, Control.ClientRectangle);

                Rectangle pad = Control.ClientRectangle;
                pad.Inflate(-10, -10);

                if (OpenGLLogo != null)
                {
                    // max of 512x256, or 2/3 height or width; proportional
                    int imgh = Math.Min((pad.Height * 2) / 3, 128);
                    int imgw = Math.Min(Math.Min((pad.Width * 2) / 3, imgh * 2), 256);
                    imgh = Math.Min(imgh, imgw / 2);

                    Rectangle r = new Rectangle(
                        pad.Right - imgw,
                        pad.Bottom - imgh,
                        imgw, imgh);
                    CompositingQuality cq = pe.Graphics.CompositingQuality;
                    pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    pe.Graphics.DrawImage(OpenGLLogo, r);
                    pe.Graphics.CompositingQuality = cq;
                }

                GLControl cont = Control as GLControl;
                string vsync = cont.VSync ? ", VSync" : "";
                using (StringFormat fmt = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near })
                using (Font f = new Font("Microsoft Sans Serif", 16))
                using (Brush b = new SolidBrush(SystemColors.Control))
                    pe.Graphics.DrawString($"{Control.Site.Name} - v{cont.GLMajorVersion}.{cont.GLMinorVersion} ({cont.ContextFlags.ToString()}{vsync})", f, b, pad, fmt);
            }

            base.OnPaintAdornments(pe);
        }

        #endregion // ---  protected override void OnPaintAdornments(PaintEventArgs pe) ---

        #region --- protected override void PreFilterEvents(IDictionary events) ---

        // <summary>
        // Adjusts the set of events the component exposes through a <see cref="TypeDescriptor"/>.
        // </summary>
        // <param name="events">An <see cref="IDictionary"/> that contains the events for the class of the component.</param>
        // <seealso cref="FilteredEvents"/>
        protected override void PreFilterEvents(IDictionary events)
        {
            base.PreFilterEvents(events);
            foreach (var e in FilteredEvents)
            {
                if (events.Contains(e))
                    events.Remove(e);
            }
        }

        #endregion // --- protected override void PreFilterEvents(IDictionary events) ---

        #region --- protected override void PreFilterProperties(IDictionary properties) ---

        // <summary>
        // Adjusts the set of properties the component exposes through a <see cref="TypeDescriptor"/>.
        // </summary>
        // <param name="properties">An <see cref="IDictionary"/> that contains the properties for the class of the component.</param>
        // <seealso cref="FilteredProperties"/>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            foreach (var p in FilteredProperties)
            {
                if (properties.Contains(p))
                    properties.Remove(p);
            }
        }

        #endregion // --- protected override void PreFilterProperties(IDictionary properties) ---

        #region --- private string[] _FilteredEvents ---

        // <summary>
        // The events that will be filtered out from the designer experience for this control.
        // </summary>
        // <seealso cref="FilteredEvents"/>
        // <seealso cref="PreFilterEvents(IDictionary)"/>
        private string[] _FilteredEvents = new string[]
        {
            "BackColorChanged",
            "BackgroundImageChanged",
            "BackgroundImageLayoutChanged",
            "ControlAdded",
            "ControlRemoved",
            "EnabledChanged",
            "FontChanged",
            "ForeColorChanged",
            "Layout",
            "PaddingChanged",
            "RightToLeftChanged",
            "SystemColorsChanged",
            "TextChanged",
            "VisibleChanged"
        };

        #endregion // --- private string[] _FilteredEvents ---

        #region --- private string[] _FilteredProperties ---

        // <summary>
        // The property names that will be filtered out from the designer experience for this control.
        // </summary>
        // <seealso cref="FilteredProperties"/>
        // <seealso cref="PreFilterProperties(IDictionary)"/>
        private string[] _FilteredProperties = new string[]
        {
            "AutoScroll",
            "AutoScrollMargin",
            "AutoScrollMinSize",
            "AutoScrollOffset",
            "AutoValidate",
            "BackColor",
            "BackgroundImage",
            "BackgroundImageLayout",
            "CausesValidation",
            "Enabled",
            "Font",
            "ForeColor",
            "Padding",
            "RightToLeft",
            "Text",
            "Visible"
        };

        #endregion // --- private string[] _FilteredProperties ---

        private Image OpenGLLogo;

        #region --- private void ComponentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e) ---

        // <summary>
        // <see cref="IComponentChangeService.ComponentChanged"/> event handler.
        // </summary>
        private void ComponentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            Control.Invalidate();
        }

        #endregion --- private void ComponentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e) ---
    }
}
