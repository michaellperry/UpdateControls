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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UpdateControls.Themes.Solid
{
    public partial class SolidDescriptorEditorForm : Form
    {
        private SolidDescriptor _value;

        private SolidCache _tileCache;
        private SolidCache _tabCache;
        private SolidCache _ellipseCache;

        private Independent _dynSize = new Independent();
        private Independent _dynDescriptor = new Independent();
        private Independent _dynSelectedPreview = new Independent();
        private Dependent _depTileImage;
        private Dependent _depTabImage;
        private Dependent _depEllipseImage;

        public SolidDescriptorEditorForm()
        {
            InitializeComponent();

            _value = DefaultTheme.DefaultNormalRegular;
            SetControls();

            _tileCache = new SolidCache(SolidShapeTile.Instance, GetTileSize, GetDescriptor);
            _tabCache = new SolidCache(SolidShapeTab.NorthInstance, GetTabSize, GetDescriptor);
            _ellipseCache = new SolidCache(SolidShapeEllipse.Instance, GetEllipseSize, GetDescriptor);

            _depTileImage = new Dependent(UpdateTileImage);
            _depTabImage = new Dependent(UpdateTabImage);
            _depEllipseImage = new Dependent(UpdateEllipseImage);
        }

        public SolidDescriptor Value
        {
            get
            {
                _dynDescriptor.OnGet();
                return _value;
            }
            set
            {
                _value = value;
                SetControls();
                ValueChanged();
            }
        }

        private void SetControls()
        {
            ambienceTrackBar.Value = (int)Math.Round(_value.Ambience * 100.0);
            aperatureTrackBar.Value = (int)Math.Round(_value.Aperature * 100.0);
            colorPictureBox.BackColor = _value.Color;
            thicknessTrackBar.Value = (int)Math.Round(_value.Thickness * 100.0);
            refractionTrackBar.Value = (int)Math.Round(_value.Refraction * 100.0);
            sharpnessTrackBar.Value = (int)Math.Round(_value.Sharpness * 100.0);
            transparentCheckBox.Checked = _value.Transparent;
        }

        private Size GetTileSize()
        {
            _dynSize.OnGet();
            return tilePreviewPictureBox.Size;
        }

        private Size GetTabSize()
        {
            _dynSize.OnGet();
            return tabPreviewPictureBox.Size;
        }

        private Size GetEllipseSize()
        {
            _dynSize.OnGet();
            return ellipsePreviewPictureBox.Size;
        }

        private SolidDescriptor GetDescriptor()
        {
            _dynDescriptor.OnGet();
            return _value;
        }

        private void directionPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                SetDirection(e.Location);
        }

        private void directionPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button &MouseButtons.Left) == MouseButtons.Left)
                SetDirection(e.Location);
        }

        private void SetDirection(Point mouse)
        {
            // Convert to floating point.
            PointF m = new PointF(
                (float)mouse.X / (float)directionPictureBox.Width * 2.0f - 1.0f,
                (float)mouse.Y / (float)directionPictureBox.Height * 2.0f - 1.0f);

            // Limit to the unit circle.
            float l2 = m.X * m.X + m.Y * m.Y;
            if (l2 > 1.0f)
            {
                float length = (float)Math.Sqrt(l2);
                m.X /= length;
                m.Y /= length;
            }

            _value.Direction = m;
            ValueChanged();
        }

        private void ambienceTrackBar_Scroll(object sender, EventArgs e)
        {
            _value.Ambience = ambienceTrackBar.Value / 100.0f;
            ValueChanged();
        }

        private void aperatureTrackBar_Scroll(object sender, EventArgs e)
        {
            _value.Aperature = aperatureTrackBar.Value / 100.0f;
            ValueChanged();
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            colorDialog.Color = _value.Color;
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                _value.Color = colorDialog.Color;
                colorPictureBox.BackColor = _value.Color;
                ValueChanged();
            }
        }

        private void thicknessTrackBar_Scroll(object sender, EventArgs e)
        {
            _value.Thickness = thicknessTrackBar.Value / 100.0f;
            ValueChanged();
        }

        private void refractionTrackBar_Scroll(object sender, EventArgs e)
        {
            _value.Refraction = refractionTrackBar.Value / 100.0f;
            ValueChanged();
        }

        private void sharpnessTrackBar_Scroll(object sender, EventArgs e)
        {
            _value.Sharpness = sharpnessTrackBar.Value / 100.0f;
            ValueChanged();
        }

        private void transparentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _value.Transparent = transparentCheckBox.Checked;
            refractionTrackBar.Enabled = _value.Transparent;
            ValueChanged();
        }

        private void ValueChanged()
        {
            _dynDescriptor.OnSet();
            directionPictureBox.Invalidate();
        }

        private void UpdateTileImage()
        {
            _dynSelectedPreview.OnGet();
            if (previewTabControl.SelectedTab == tileTabPage)
                tilePreviewPictureBox.Image = _tileCache.Image;
        }

        private void UpdateTabImage()
        {
            _dynSelectedPreview.OnGet();
            if (previewTabControl.SelectedTab == tabTabPage)
                tabPreviewPictureBox.Image = _tabCache.Image;
        }

        private void UpdateEllipseImage()
        {
            _dynSelectedPreview.OnGet();
            if (previewTabControl.SelectedTab == ellipseTabPage)
                ellipsePreviewPictureBox.Image = _ellipseCache.Image;
        }

        protected override void OnLoad(EventArgs e)
        {
            Application.Idle += new EventHandler(Application_Idle);
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            if (_depTileImage != null)
                _depTileImage.Dispose();
            if (_depTabImage != null)
                _depTabImage.Dispose();
            if (_depEllipseImage != null)
                _depEllipseImage.Dispose();
            if (_tileCache != null)
                _tileCache.Dispose();
            if (_tabCache != null)
                _tabCache.Dispose();
            if (_ellipseCache != null)
                _ellipseCache.Dispose();
            base.OnClosed(e);
        }

        void Application_Idle(object sender, EventArgs e)
        {
            _depTileImage.OnGet();
            _depTabImage.OnGet();
            _depEllipseImage.OnGet();
        }

        private void directionPictureBox_Paint(object sender, PaintEventArgs e)
        {
            // Draw the unit circle.
            Pen pen = new Pen(Color.Black);
            Rectangle bounds = directionPictureBox.ClientRectangle;
            bounds.X += 5;
            bounds.Y += 5;
            bounds.Width -= 10;
            bounds.Height -= 10;
            e.Graphics.DrawEllipse(pen, bounds);

            // Draw the light vector.
            Point center = new Point(
                (int)((_value.Direction.X + 1.0f) * (float)bounds.Width * 0.5f) + bounds.X,
                (int)((_value.Direction.Y + 1.0f) * (float)bounds.Height * 0.5f) + bounds.Y);
            e.Graphics.DrawLine(pen, center.X - 5, center.Y - 5, center.X + 5, center.Y + 5);
            e.Graphics.DrawLine(pen, center.X + 5, center.Y - 5, center.X - 5, center.Y + 5);
        }

        private void ImageResize(object sender, EventArgs e)
        {
            _dynSize.OnSet();
        }

        private void idleTimer_Tick(object sender, EventArgs e)
        {
            Application.RaiseIdle(new EventArgs());
        }

        private void previewTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _dynSelectedPreview.OnSet();
        }
    }
}