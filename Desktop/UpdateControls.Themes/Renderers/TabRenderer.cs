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
using UpdateControls.Themes.Forms;
using System.Drawing;
using System.Windows.Forms;
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes.Renderers
{
    public class TabRenderer : Renderer, IDisposable
    {
        #region Construction

        public TabRenderer(Context context, object item, OrientationType orientation, bool selected, bool ownsImage)
        {
            _context = context;
            _item = item;
            _orientation = orientation;
            _selected = selected;
            _ownsImage = ownsImage;

            // Create the image cache based on the orientation.
            SolidShape shape =
                orientation == OrientationType.Top     ?   SolidShapeTab.NorthInstance :
                orientation == OrientationType.Left    ?   SolidShapeTab.WestInstance :
                orientation == OrientationType.Bottom  ?   SolidShapeTab.SouthInstance :
                /*orientation == OrientationType.Right ?*/ SolidShapeTab.EastInstance;
            _image = new SolidCache(shape, GetTabSize, GetTabDescriptor);

            // Dependent properties.
            _depEmbeddedImage = new Dependent(UpdateEmbeddedImage);
            _depText = new Dependent(UpdateText);
            _depBounds = new Dependent(UpdateBounds);
            _depImage = new Dependent(UpdateImage);
        }

        #endregion

        #region Context

        public interface Context
        {
            string GetText(object item);
            bool HasImages { get; }
            Image GetImage(object item);
            Theme Theme { get; }
            Theme CloseTheme { get; }
            Size GetSize(object item);
            Rectangle GetBounds(object item);

            void SelectItem(object sitem);
            void DragItem(object item, Point start, Point position);
            void EndDrag();

            bool CanClose { get; }
            void OnClickClose(object item);

            void InvalidateRectangle(Rectangle rectangle);
        }

        private Context _context;
        private object _item;
        private OrientationType _orientation;
        private bool _selected;

        #endregion

        #region Image of the tab

        private SolidCache _image;
        private bool _ownsImage;
        private Image _embeddedImage;
        private string _text;
        private Rectangle _bounds;
        private Dependent _depEmbeddedImage;
        private Dependent _depText;
        private Dependent _depBounds;
        private Dependent _depImage;

        private Size GetTabSize()
        {
            return _context.GetSize(_item);
        }

        private SolidDescriptor GetTabDescriptor()
        {
            if (_selected)
                return DefaultTheme.SolidFocused(_context.Theme);
            else
                return DefaultTheme.SolidRegular(_context.Theme);
        }

        private void UpdateEmbeddedImage()
        {
            if (_ownsImage && _embeddedImage != null)
                _embeddedImage.Dispose();
            _context.InvalidateRectangle(_bounds);
            _embeddedImage = _context.GetImage(_item);
        }

        private void UpdateText()
        {
            _context.InvalidateRectangle(_bounds);
            _text = _context.GetText(_item);
        }

        private void UpdateBounds()
        {
            Rectangle oldBounds = _bounds;
            _bounds = _context.GetBounds(_item);

            if (!oldBounds.Equals(_bounds))
            {
                // Invalidate the old and new bounds.
                _context.InvalidateRectangle(oldBounds);
                _context.InvalidateRectangle(_bounds);
            }
        }

        private void UpdateImage()
        {
            _context.InvalidateRectangle(_bounds);
            Image dummy = _image.Image;
        }

        public override void OnIdle()
        {
            _depEmbeddedImage.OnGet();
            _depText.OnGet();
            _depBounds.OnGet();
            _depImage.OnGet();
            base.OnIdle();
        }

        #endregion

        #region Input

        public override bool HitTest(Point mouse)
        {
            return _context.GetBounds(_item).Contains(mouse);
        }

        public override void OnMouseDown(Point mouse, MouseButtons button, int clicks, Keys modifierKeys)
        {
            _context.SelectItem(_item);
            base.OnMouseDown(mouse, button, clicks, modifierKeys);
        }

        public override void OnMouseDrag(Point start, Point mouse)
        {
            _context.DragItem(_item, start, mouse);
            base.OnMouseDrag(start, mouse);
        }

        public override void OnMouseUp(Point start, Point mouse)
        {
            _context.EndDrag();
            base.OnMouseUp(start, mouse);
        }

        #endregion

        #region Painting

        public override void PaintBackground(System.Drawing.Graphics graphics)
        {
            using (new GraphicsUtil.GraphicsStateSaver(graphics))
            {
                // Paint the tab image.
                graphics.DrawImage(_image.Image, _bounds.Left, _bounds.Top);

                // Paint the embedded image.
                if (_embeddedImage != null)
                    graphics.DrawImage(_embeddedImage, EmbeddedImageRectangle);

                // Paint the text.
                Rectangle bounds;
                if (_orientation == OrientationType.Top || _orientation == OrientationType.Bottom)
                {
                    bounds = _bounds;
                }
                else if (_orientation == OrientationType.Left)
                {
                    GraphicsUtil.GraphicsUtil.RotateGraphicsCCW(graphics);
                    bounds = GraphicsUtil.GraphicsUtil.RotateRectangleCCW(_bounds);
                }
                else
                {
                    GraphicsUtil.GraphicsUtil.RotateGraphicsCW(graphics);
                    bounds = GraphicsUtil.GraphicsUtil.RotateRectangleCW(_bounds);
                }

                // Reserve space for the edges and the close button.
                bounds.X += bounds.Height / 4;
                bounds.Width -= bounds.Height / 2;
                if (_context.CanClose)
                {
                    bounds.Width -= bounds.Height;
                }
                if (_context.HasImages)
                {
                    bounds.X += bounds.Height;
                    bounds.Width -= bounds.Height;
                }
                Theme theme = _context.Theme;
                Font font = DefaultTheme.FontHeader(theme);
                Color color = DefaultTheme.ColorBody(theme);
                StringFormat stringFormat = new StringFormat(StringFormatFlags.MeasureTrailingSpaces);
                stringFormat.Alignment = StringAlignment.Center;
                graphics.DrawString(
                    _text,
                    font,
                    new SolidBrush(color),
                    bounds,
                    stringFormat);
            }

            base.PaintBackground(graphics);
        }

        #endregion

        #region Child Renderers

        #region CloseButtonRendererContext

        private class CloseButtonRendererContext : ThemedButtonRenderer.Context
        {
            private TabRenderer _owner;

            public CloseButtonRendererContext(TabRenderer owner)
            {
                _owner = owner;
            }

            public override Theme Theme
            {
                get { return _owner._context.CloseTheme; }
            }

            public override DefaultTheme.Style Style
            {
                get { return DefaultTheme.Style.Close; }
            }

            public override Rectangle Rectangle
            {
                get { return _owner.CloseButtonRectangle; }
            }

            public override Size Size
            {
                get { return _owner.CloseButtonSize; }
            }

            public override bool Enabled
            {
                get { return true; }
            }

            public override bool Focused
            {
                get { return false; }
            }

            public override string Text
            {
                get { return string.Empty; }
            }

            public override void PaintButton(Graphics graphics, Rectangle bounds)
            {
                _owner.PaintCloseButton(graphics, bounds);
            }

            public override void InvalidateRectangle(Rectangle rect)
            {
                _owner._context.InvalidateRectangle(rect);
            }

            public override void Click()
            {
                _owner.OnClickClose();
            }
        }
        #endregion

        protected override IEnumerable<Renderer> GetChildRenderers()
        {
            // Add the close button.
            if (_context.CanClose)
                yield return new ThemedButtonRenderer(new CloseButtonRendererContext(this), SolidShapeRoundedRectangle.Instance);
        }

        private Rectangle CloseButtonRectangle
        {
            get
            {
                _depBounds.OnGet();

                Rectangle closeButtonBounds = _bounds;

                // Position the close button on the right side of the text.
                if (_orientation == OrientationType.Top || _orientation == OrientationType.Bottom)
                {
                    int border = -3 * closeButtonBounds.Height / 16;
                    closeButtonBounds.X += closeButtonBounds.Width - closeButtonBounds.Height - closeButtonBounds.Height / 4;
                    closeButtonBounds.Width = closeButtonBounds.Height;
                    closeButtonBounds.Inflate(border, border);
                }
                else if (_orientation == OrientationType.Left)
                {
                    int border = -3 * closeButtonBounds.Width / 16;
                    closeButtonBounds.Y += closeButtonBounds.Width / 4;
                    closeButtonBounds.Height = closeButtonBounds.Width;
                    closeButtonBounds.Inflate(border, border);
                }
                else /*if (_orientation == OrientationType.Right)*/
                {
                    int border = -3 * closeButtonBounds.Width / 16;
                    closeButtonBounds.Y += closeButtonBounds.Height - closeButtonBounds.Width - closeButtonBounds.Width / 4;
                    closeButtonBounds.Height = closeButtonBounds.Width;
                    closeButtonBounds.Inflate(border, border);
                }
                return closeButtonBounds;
            }
        }

        private Rectangle EmbeddedImageRectangle
        {
            get
            {
                _depBounds.OnGet();

                Rectangle embeddedImageBounds = _bounds;

                // Position the close button on the right side of the text.
                if (_orientation == OrientationType.Top || _orientation == OrientationType.Bottom)
                {
                    int border = -3 * embeddedImageBounds.Height / 16;
                    embeddedImageBounds.X += embeddedImageBounds.Height / 4;
                    embeddedImageBounds.Width = embeddedImageBounds.Height;
                    embeddedImageBounds.Inflate(border, border);
                }
                else if (_orientation == OrientationType.Left)
                {
                    int border = -3 * embeddedImageBounds.Width / 16;
                    embeddedImageBounds.Y += embeddedImageBounds.Height - embeddedImageBounds.Width - embeddedImageBounds.Width / 4;
                    embeddedImageBounds.Height = embeddedImageBounds.Width;
                    embeddedImageBounds.Inflate(border, border);
                }
                else /*if (_orientation == OrientationType.Right)*/
                {
                    int border = -3 * embeddedImageBounds.Width / 16;
                    embeddedImageBounds.Y += embeddedImageBounds.Width / 4;
                    embeddedImageBounds.Height = embeddedImageBounds.Width;
                    embeddedImageBounds.Inflate(border, border);
                }
                return embeddedImageBounds;
            }
        }

        private Size CloseButtonSize
        {
            get
            {
                // Get the item size.
                Size size = _context.GetSize(_item);

                // Shrink the close button to a fraction of the vertical direction.
                if (_orientation == OrientationType.Top || _orientation == OrientationType.Bottom)
                {
                    size.Height -= 3 * size.Height / 8;
                    size.Width = size.Height;
                }
                else /*if (_orientation == OrientationType.Left || _orientation == OrientationType.Right)*/
                {
                    size.Width -= 3 * size.Width / 8;
                    size.Height = size.Width;
                }

                return size;
            }
        }

        private void PaintCloseButton(Graphics graphics, Rectangle closeButtonBounds)
        {
            // Paint the X over the close button.
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.White, (float)closeButtonBounds.Width / 8.0F))
            {
                float offset = (float)closeButtonBounds.Width / 4.0F;
                graphics.DrawLine(pen,
                    (float)closeButtonBounds.Left + offset - 1.0F, (float)closeButtonBounds.Top + offset - 1.0F,
                    (float)closeButtonBounds.Right - offset - 1.0F, (float)closeButtonBounds.Bottom - offset - 1.0F);
                graphics.DrawLine(pen,
                    (float)closeButtonBounds.Right - offset - 1.0F, (float)closeButtonBounds.Top + offset - 1.0F,
                    (float)closeButtonBounds.Left + offset - 1.0F, (float)closeButtonBounds.Bottom - offset - 1.0F);
            }
        }

        private void OnClickClose()
        {
            _context.OnClickClose(_item);
        }

        #endregion

        #region Equals

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj == this)
                return true;
            if (obj.GetType() != this.GetType())
                return false;

            TabRenderer that = (TabRenderer)obj;
            return
                Object.Equals(this._item, that._item) &&
                Object.Equals(this._orientation, that._orientation) &&
                this._selected == that._selected &&
                this._ownsImage == that._ownsImage;
        }

        public override int GetHashCode()
        {
            return ((_item.GetHashCode() * 37 + _orientation.GetHashCode()) * 2 + (_selected ? 1 : 0)) * 2 + (_ownsImage ? 1 : 0);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (_ownsImage && _embeddedImage != null)
                _embeddedImage.Dispose();
            _context.InvalidateRectangle(_bounds);
            _depEmbeddedImage.Dispose();
            _depText.Dispose();
            _depBounds.Dispose();
            _depImage.Dispose();
            _image.Dispose();
        }

        #endregion
    }
}
