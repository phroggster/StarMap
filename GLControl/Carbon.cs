#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library, except where noted.
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
using System.Drawing;
using System.Runtime.InteropServices;

namespace Phroggiesoft.Controls.Platform.MacOS.Carbon
{
    internal enum OSStatus
    {
        NoError = 0,

        ParameterError               = -50,                          /*error in user parameter list*/
        NoHardwareError             = -200,                         /*Sound Manager Error Returns*/
        NotEnoughHardwareError      = -201,                         /*Sound Manager Error Returns*/
        UserCanceledError           = -128,
        QueueError                  = -1,                           /*queue element not found during deletion*/
        VTypErr                     = -2,                           /*invalid queue element*/
        CorErr                      = -3,                           /*core routine number out of range*/
        UnimpErr                    = -4,                           /*unimplemented core routine*/
        SlpTypeErr                  = -5,                           /*invalid queue element*/
        SeNoDB                      = -8,                           /*no debugger installed to handle debugger command*/
        ControlErr                  = -17,                          /*I/O System Errors*/
        StatusErr                   = -18,                          /*I/O System Errors*/
        ReadErr                     = -19,                          /*I/O System Errors*/
        WritErr                     = -20,                          /*I/O System Errors*/
        BadUnitErr                  = -21,                          /*I/O System Errors*/
        UnitEmptyErr                = -22,                          /*I/O System Errors*/
        OpenErr                     = -23,                          /*I/O System Errors*/
        ClosErr                     = -24,                          /*I/O System Errors*/
        DRemovErr                   = -25,                          /*tried to remove an open driver*/
        DInstErr                    = -26,                          /*DrvrInstall couldn't find driver in resources*/

        // Window Manager result codes.
        InvalidWindowPtr = -5600,
        UnsupportedWindowAttributesForClass = -5601,
        WindowDoesNotHaveProxy = -5602,
        WindowPropertyNotFound = -5604,
        UnrecognizedWindowClass = -5605,
        CorruptWindowDescription = -5606,
        UserWantsToDragWindow = -5607,
        WindowsAlreadyInitialized = -5608,
        FloatingWindowsNotInitialized = -5609,
        WindowNotFound = -5610,
        WindowDoesNotFitOnscreen = -5611,
        WindowAttributeImmutable = -5612,
        WindowAttributesConflict = -5613,
        WindowManagerInternalError = -5614,
        WindowWrongState = -5615,
        WindowGroupInvalid = -5616,
        WindowAppModalStateAlreadyExists = -5617,
        WindowNoAppModalState = -5618,
        WindowDoesntSupportFocus = -30583,
        WindowRegionCodeInvalid = -30593,

        // Event Manager result codes
        EventAlreadyPosted = -9860,
        EventTargetBusy = -9861,
        EventDeferAccessibilityEvent = -9865,
        EventInternalError = -9868,
        EventParameterNotFound = -9870,
        EventNotHandled = -9874,
        EventLoopTimedOut = -9875,
        EventLoopQuit = -9876,
        EventNotInQueue = -9877,
        HotKeyExists = -9878,
        EventPassToNextTarget = -9880
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        short top;
        short left;
        short bottom;
        short right;

        internal Rect(int left, int top, int width, int height)
            : this((short)left, (short)top, (short)width, (short)height)
        {
        }

        internal Rect(short _left, short _top, short _width, short _height)
        {
            top = _top;
            left = _left;
            bottom = (short)(_top + _height);
            right = (short)(_left + _width);
        }

        internal short X
        {
            get { return left; }
            set
            {
                short width = Width;
                left = value;
                right = (short)(left + width);
            }
        }

        internal short Y
        {
            get { return top; }
            set
            {
                short height = Height;
                top = value;
                bottom = (short)(top + height);
            }
        }

        internal short Width 
        { 
            get { return (short)(right - left); } 
            set { right = (short)(left + value); }
        }

        internal short Height 
        { 
            get { return (short)(bottom - top); } 
            set { bottom = (short)(top + value); }
        }

        public override string ToString()
        {
            return string.Format(
                "Rect: [{0}, {1}, {2}, {3}]", X, Y, Width, Height);
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }

    internal class API
    {
        const string carbon = "/System/Library/Frameworks/Carbon.framework/Versions/Current/Carbon";

        [DllImport(carbon)]
        internal unsafe static extern OSStatus DMGetGDeviceByDisplayID(
            IntPtr displayID, out IntPtr displayDevice, Boolean failToMain);

        [DllImport(carbon)]
        static extern IntPtr GetControlBounds(IntPtr control, out Rect bounds);

        internal static Rect GetControlBounds(IntPtr control)
        {
            Rect retval;
            GetControlBounds(control, out retval);
            return retval;
        }

        [DllImport(carbon)]
        internal static extern IntPtr GetControlOwner(IntPtr control);

        [DllImport(carbon)]
        internal static extern IntPtr GetWindowPort(IntPtr windowRef);
    }
}

