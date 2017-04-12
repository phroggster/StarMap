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
using System.Diagnostics;
using System.Windows.Forms;
#endregion // --- using ... ---

namespace StarMap
{
    internal class StatusStripParentResizeGrip : StatusStrip
    {
        private const int HTTRANSPARENT = -1;
        private const int WM_NCHITTEST = 0x0084;

        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST && SizingGrip)
            {
                var pos = PointToClient(Cursor.Position);
                // if (distance from right) plus (distance from bottom) is less than height,
                // then we must be within the grip area. Mark it transparent to pass the buck.
                if (ClientRectangle.Right - pos.X + ClientRectangle.Bottom - pos.Y < ClientRectangle.Height)
                {
                    m.Result = (IntPtr)HTTRANSPARENT;
                    return;
                }
            }
            base.WndProc(ref m);
        }
    }
}
