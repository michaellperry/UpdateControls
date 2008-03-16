/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
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
using UpdateControls.Themes.Solid;

namespace UpdateControls.Themes
{
    public static class DefaultTheme
    {
        public static readonly Color DefaultColorHeader = Color.Black;
        public static readonly Color DefaultColorBody = Color.Black;
        public static readonly Color DefaultColorEven = Color.Snow;
        public static readonly Color DefaultColorOdd = Color.WhiteSmoke;
        public static readonly Color DefaultColorSelected = Color.BlanchedAlmond;

        public static readonly Font DefaultFontHeader = new Font(FontFamily.GenericSansSerif, 12.0f, FontStyle.Bold);
        public static readonly Font DefaultFontBody = new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Regular);

        public static readonly SolidDescriptor DefaultNormalDisabled = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(196, 196, 196), 0.15f, 0.4f, 0.96f, false);
        public static readonly SolidDescriptor DefaultNormalRegular = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(128, 128, 255), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultNormalHover = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.8f, 0.08f, Color.FromArgb(128, 128, 255), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultNormalFocused = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(255, 128, 255), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultNormalPressed = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(255, 128, 255), 0.05f, 0.4f, 0.96f, true);

        public static readonly SolidDescriptor DefaultUncheckedDisabled = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(196, 196, 196), 0.15f, 0.4f, 0.96f, false);
        public static readonly SolidDescriptor DefaultUncheckedRegular = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(255, 255, 255), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultUncheckedHover = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.8f, 0.08f, Color.FromArgb(230, 218, 231), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultUncheckedFocused = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(245, 222, 248), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultUncheckedPressed = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(226, 207, 226), 0.05f, 0.4f, 0.96f, true);

        public static readonly SolidDescriptor DefaultCloseDisabled = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(196, 196, 196), 0.15f, 0.4f, 0.96f, false);
        public static readonly SolidDescriptor DefaultCloseRegular = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(255, 60, 60), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultCloseHover = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.8f, 0.08f, Color.FromArgb(255, 80, 80), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultCloseFocused = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(255, 60, 60), 0.15f, 0.4f, 0.96f, true);
        public static readonly SolidDescriptor DefaultClosePressed = new SolidDescriptor(new PointF(-0.42f, -0.54f), 0.6f, 0.08f, Color.FromArgb(255, 80, 80), 0.05f, 0.4f, 0.96f, true);

        public enum Style
        {
            Normal,
            Unchecked,
            Close
        }

        public static void OnGet(Theme theme)
        {
            if (theme != null)
            {
                Font dummyFont;
                dummyFont = theme.FontHeader;
                dummyFont = theme.FontBody;

                Color dummyColor;
                dummyColor = theme.ColorHeader;
                dummyColor = theme.ColorBody;
                dummyColor = theme.ColorEven;
                dummyColor = theme.ColorOdd;
                dummyColor = theme.ColorSelected;
            }
        }

        public static Font FontBody(Theme theme)
        {
            if (theme != null)
                return theme.FontBody;
            else
                return DefaultFontBody;
        }

        public static Font FontHeader(Theme theme)
        {
            if (theme != null)
                return theme.FontHeader;
            else
                return DefaultFontHeader;
        }

        public static Color ColorBody(Theme theme)
        {
            if (theme != null)
                return theme.ColorBody;
            else
                return DefaultColorBody;
        }

        public static Color ColorHeader(Theme theme)
        {
            if (theme != null)
                return theme.ColorHeader;
            else
                return DefaultColorHeader;
        }

        public static SolidDescriptor SolidRegular(Theme theme, Style style)
        {
            if (theme != null)
            {
                return theme.SolidRegular.ApparentValue;
            }
            else
            {
                if (style == Style.Normal)
                    return DefaultNormalRegular;
                else if (style == Style.Unchecked)
                    return DefaultUncheckedRegular;
                else /*if (style == Style.Close)*/
                    return DefaultCloseRegular;
            }
        }

        public static SolidDescriptor SolidDisabled(Theme theme, Style style)
        {
            if (theme != null)
            {
                return theme.SolidDisabled.ApparentValue;
            }
            else
            {
                if (style == Style.Normal)
                    return DefaultNormalDisabled;
                else if (style == Style.Unchecked)
                    return DefaultUncheckedDisabled;
                else /*if (style == Style.Close)*/
                    return DefaultCloseDisabled;
            }
        }

        public static SolidDescriptor SolidFocused(Theme theme, Style style)
        {
            if (theme != null)
            {
                return theme.SolidFocused.ApparentValue;
            }
            else
            {
                if (style == Style.Normal)
                    return DefaultNormalFocused;
                else if (style == Style.Unchecked)
                    return DefaultUncheckedFocused;
                else /*if (style == Style.Close)*/
                    return DefaultCloseFocused;
            }
        }

        public static SolidDescriptor SolidHover(Theme theme, Style style)
        {
            if (theme != null)
            {
                return theme.SolidHover.ApparentValue;
            }
            else
            {
                if (style == Style.Normal)
                    return DefaultNormalHover;
                else if (style == Style.Unchecked)
                    return DefaultUncheckedHover;
                else /*if (style == Style.Close)*/
                    return DefaultCloseHover;
            }
        }

        public static SolidDescriptor SolidPressed(Theme theme, Style style)
        {
            if (theme != null)
            {
                return theme.SolidPressed.ApparentValue;
            }
            else
            {
                if (style == Style.Normal)
                    return DefaultNormalPressed;
                else if (style == Style.Unchecked)
                    return DefaultUncheckedPressed;
                else /*if (style == Style.Close)*/
                    return DefaultClosePressed;
            }
        }

        public static SolidDescriptor SolidRegular(Theme theme)
        {
            return SolidRegular(theme, Style.Normal);
        }

        public static SolidDescriptor SolidDisabled(Theme theme)
        {
            return SolidDisabled(theme, Style.Normal);
        }

        public static SolidDescriptor SolidFocused(Theme theme)
        {
            return SolidFocused(theme, Style.Normal);
        }

        public static SolidDescriptor SolidHover(Theme theme)
        {
            return SolidHover(theme, Style.Normal);
        }

        public static SolidDescriptor SolidPressed(Theme theme)
        {
            return SolidPressed(theme, Style.Normal);
        }

        public static Color ColorEven(Theme theme)
        {
            if (theme != null)
            {
                return theme.ColorEven;
            }
            else
            {
                return DefaultColorEven;
            }
        }

        public static Color ColorOdd(Theme theme)
        {
            if (theme != null)
            {
                return theme.ColorOdd;
            }
            else
            {
                return DefaultColorOdd;
            }
        }

        public static Color ColorSelected(Theme theme)
        {
            if (theme != null)
            {
                return theme.ColorSelected;
            }
            else
            {
                return DefaultColorSelected;
            }
        }
    }
}
