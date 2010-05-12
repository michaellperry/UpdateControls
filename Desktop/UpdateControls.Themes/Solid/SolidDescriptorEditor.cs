/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace UpdateControls.Themes.Solid
{
    public class SolidDescriptorEditor : UITypeEditor
    {
        private SolidDescriptorEditorForm _form = null;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;
                if (editorService != null)
                {
                    _form = new SolidDescriptorEditorForm();
                    SolidDescriptorEditable editable = (SolidDescriptorEditable)value;
                    _form.Value = editable.Value;
                    editable.Attach(this);
                    if (editorService.ShowDialog(_form) == DialogResult.OK)
                        value = new SolidDescriptorEditable(_form.Value);
                    editable.Detach(this);
                    _form = null;

                }
            }
            return value;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            using (SolidCache cache = new SolidCache(SolidShapeTile.Instance, delegate()
            {
                return e.Bounds.Size;
            }, delegate()
            {
                return ((SolidDescriptorEditable)e.Value).Value;
            }))
            {
                e.Graphics.DrawImage(cache.Image, new System.Drawing.Point(0, 0));
            }
        }

        public SolidDescriptor WorkingValue
        {
            get
            {
                return _form.Value;
            }
        }
    }
}
