#region License
//
// The Open Toolkit Library License
//
// Author:
//       Former CodeProject user 'LogicNP'
//  https://www.codeproject.com/Articles/13793/A-UITypeEditor-for-easy-editing-of-flag-enum-prope
//
//       With modifications by phroggie <phroggster@gmail.com>
//
// Copyright (c) 2006, 2017
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
#endregion

using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

// Attribution:
// With special thanks to 
// ****************************

namespace Phroggiesoft.Controls
{
    internal class FlagsListBox : CheckedListBox
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Enum EnumValue
        {
            get
            {
                return Enum.ToObject(enumType, GetCurrentValue()) as Enum;
            }
            set
            {
                Items.Clear();
                enumValue = value;
                enumType = value.GetType();
                Populate();
                ApplyEnumValue();
            }
        }

        public FlagsListBox()
        {
            SuspendLayout();
            CheckOnClick = true;
            ResumeLayout(false);
        }

        public FlagsListBoxItem Add(int v, string c)
        {
            FlagsListBoxItem item = new FlagsListBoxItem(v, c);
            return Add(item);
        }

        public FlagsListBoxItem Add(FlagsListBoxItem item)
        {
            Items.Add(item);
            return item;
        }

        public int GetCurrentValue()
        {
            int sum = 0;

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i] as FlagsListBoxItem;
                if (GetItemChecked(i))
                    sum |= item.Value;
            }

            return sum;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            base.OnItemCheck(ice);

            if (suspendUpdates)
                return;

            FlagsListBoxItem item = Items[ice.Index] as FlagsListBoxItem;
            UpdateCheckedItems(item, ice.NewValue);
        }

        protected void InnerUpdateCheckedItems(int value)
        {
            suspendUpdates = true;

            for (int i = 0; i < Items.Count; i++)
            {
                FlagsListBoxItem item = Items[i] as FlagsListBoxItem;

                if (item.Value == 0)
                    SetItemChecked(i, (value == 0));
                else
                    SetItemChecked(i, ((item.Value & value) == item.Value && item.Value != 0));
            }

            suspendUpdates = false;
        }

        protected void UpdateCheckedItems(FlagsListBoxItem items, CheckState cs)
        {
            if (items.Value == 0)
                InnerUpdateCheckedItems(0);

            int sum = GetCurrentValue();
            if (cs == CheckState.Unchecked)
                sum = sum & ~items.Value;
            else
                sum |= items.Value;

            InnerUpdateCheckedItems(sum);
        }


        private IContainer components = null;
        private Type enumType;
        private Enum enumValue;
        private bool suspendUpdates = false;

        private void ApplyEnumValue()
        {
            InnerUpdateCheckedItems((int)Convert.ChangeType(enumValue, typeof(int)));
        }

        private void Populate()
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                var val = Enum.Parse(enumType, name);
                var intVal = (int)Convert.ChangeType(val, typeof(int));
                Add(intVal, name);
            }
        }
    }

    internal class FlagsListBoxItem
    {
        public string Caption { get; private set; }
        public int Value { get; private set; }

        public bool ContainsFlag
        {
            get
            {
                return ((Value & (Value - 1)) == 0);
            }
        }

        public FlagsListBoxItem(int v, string c)
        {
            Caption = c;
            Value = v;
        }

        public override string ToString()
        {
            return Caption;
        }
    }

    internal class FlagsListBoxUIEditor : UITypeEditor
    {
        private FlagsListBox _flagsLB;

        public FlagsListBoxUIEditor()
        {
            _flagsLB = new FlagsListBox();
            _flagsLB.BorderStyle = BorderStyle.None;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                IWindowsFormsEditorService wfes = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (wfes != null)
                {
                    Enum e = (Enum)Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
                    _flagsLB.EnumValue = e;
                    wfes.DropDownControl(_flagsLB);
                    return _flagsLB.EnumValue;
                }
            }
            return null;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
