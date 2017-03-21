/*
 * Copyright © 2017 phroggie <phroggster@gmail.com>, StarMap development team
 * Copyright © 2006 CodeProject user 'LogicNP'
 *   https://www.codeproject.com/Articles/13793/A-UITypeEditor-for-easy-editing-of-flag-enum-prope
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
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Phroggiesoft.EditorControls
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

    public class FlagsListBoxUIEditor : UITypeEditor
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
