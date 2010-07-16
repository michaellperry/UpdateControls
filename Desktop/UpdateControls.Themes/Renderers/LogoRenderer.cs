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
using UpdateControls.Themes.Forms;
using System.Drawing.Drawing2D;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes.Renderers
{
    public class LogoRenderer : Renderer
    {
        public interface Context
        {
            Theme Theme { get; }
            Color HelixColor { get; }
            Rectangle Rectangle { get; }
            void Invalidate(Rectangle rectangle);
        }

        private Context _context;

        private SolidCache _normalSolid;

        private Image _image;
        private Color _helixColor;
        private Rectangle _rectangle = new Rectangle();
        private Dependent _depImage;
        private Dependent _depRectangle;

        private static readonly SolidDescriptor _defaultLogoSolidDescriptor = new SolidDescriptor(new PointF(-0.4f, -0.82f), 0.57f, 0.1f, Color.FromArgb(70, 208, 87), 0.83f, 0.97f, 0.83f, true);

        public LogoRenderer(Context context)
        {
            _context = context;
            _normalSolid = new SolidCache(SolidShapeSphere.Instance, GetSize, GetNormalDescriptor);

            _depImage = new Dependent(UpdateImage);
            _depRectangle = new Dependent(UpdateRectangle);
        }

        private Size GetSize()
        {
            return _context.Rectangle.Size;
        }

        private SolidDescriptor GetNormalDescriptor()
        {
            if (_context.Theme != null)
                return _context.Theme.SolidRegular.ApparentValue;
            else
                return _defaultLogoSolidDescriptor;
        }

        private void UpdateImage()
        {
            _image = _normalSolid.Image;
            _helixColor = _context.HelixColor;
            if (!_rectangle.IsEmpty)
                _context.Invalidate(_rectangle);
        }

        private void UpdateRectangle()
        {
            if (!_rectangle.IsEmpty)
                _context.Invalidate(_rectangle);
            _rectangle = _context.Rectangle;
            _context.Invalidate(_rectangle);
        }

        public override void PaintForeground(System.Drawing.Graphics graphics)
        {
            _depImage.OnGet();
            _depRectangle.OnGet();
            if (_image != null)
                graphics.DrawImage(_image, _rectangle.Location);

            // Paint the arrow on the sphere.
            float x = (float)_rectangle.Width;
            float y = (float)_rectangle.Height;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Brush brush = new SolidBrush(_helixColor))
            {
                using (GraphicsPath helix = new GraphicsPath())
                {
                    helix.AddBezier(0.282f * x, 0.954f * y, 0.522f * x, 0.920f * y, 0.728f * x, 0.856f * y, 0.950f * x, 0.720f * y);
                    helix.AddBezier(0.964f * x, 0.690f * y, 0.752f * x, 0.824f * y, 0.558f * x, 0.898f * y, 0.266f * x, 0.946f * y);
                    helix.CloseFigure();
                    graphics.FillPath(brush, helix);
                }
                using (GraphicsPath helix = new GraphicsPath())
                {
                    helix.AddBezier(0.014f * x, 0.636f * y, 0.412f * x, 0.614f * y, 0.754f * x, 0.440f * y, 0.926f * x, 0.146f * y);
                    helix.AddLine(0.944f * x, 0.162f * y, 0.970f * x, 0.030f * y);
                    helix.AddLine(0.970f * x, 0.030f * y, 0.856f * x, 0.092f * y);
                    helix.AddBezier(0.876f * x, 0.108f * y, 0.656f * x, 0.426f * y, 0.378f * x, 0.570f * y, 0.002f * x, 0.588f * y);
                    helix.CloseFigure();
                    graphics.FillPath(brush, helix);
                }
            }

            base.PaintForeground(graphics);
        }

        public override void OnIdle()
        {
            _depImage.OnGet();
            _depRectangle.OnGet();

            base.OnIdle();
        }

        public override void Dispose()
        {
            _depImage.Dispose();
            _depRectangle.Dispose();
            _normalSolid.Dispose();

            base.Dispose();
        }
    }
}
