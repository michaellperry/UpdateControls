/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes
{
    [Serializable]
    public class SolidDescriptorEditable
    {
        private SolidDescriptor _value;

        [NonSerialized]
        private SolidDescriptorEditor _editor = null;

        [NonSerialized]
        Independent _dynEditor = new Independent();

        public SolidDescriptorEditable()
        {
            _value = DefaultTheme.DefaultNormalRegular;
        }

        public SolidDescriptorEditable(SolidDescriptor value)
        {
            _value = value;
        }

        public SolidDescriptor Value
        {
            get { return _value; }
        }

        public void Attach(SolidDescriptorEditor editor)
        {
            System.Diagnostics.Debug.Assert(_editor == null);
            if (_dynEditor == null) _dynEditor = new Independent();
            _dynEditor.OnSet();
            _editor = editor;
        }

        public void Detach(SolidDescriptorEditor editor)
        {
            System.Diagnostics.Debug.Assert(_editor == editor);
            if (_dynEditor == null) _dynEditor = new Independent();
            _dynEditor.OnSet();
            _editor = null;
        }

        public SolidDescriptor ApparentValue
        {
            get
            {
                if (_dynEditor == null) _dynEditor = new Independent();
                _dynEditor.OnGet();
                if (_editor != null)
                    return _editor.WorkingValue;
                else
                    return Value;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is SolidDescriptorEditable))
                return false;
            return _value.Equals(((SolidDescriptorEditable)obj)._value);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
