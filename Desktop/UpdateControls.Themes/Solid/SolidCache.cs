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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace UpdateControls.Themes.Solid
{
    public class SolidCache : IDisposable
    {
        public delegate SolidDescriptor GetDescriptorDelegate();
        public delegate Size GetSizeDelegate();

        private GetDescriptorDelegate _getDescriptor;
        private GetSizeDelegate _getSize;

        private SolidShape _shape;
        private Size _size = new Size(0,0);
        private Size _previousSize = new Size(0, 0);
        private SolidDescriptor _descriptor;
        private SolidDescriptor _previousDescriptor;

        private Bitmap _image;
        private IntPtr _imageMemory;

        private Dependent _depSize;
        private Dependent _depDescriptor;
        private Dependent _depImage;

        public SolidCache(SolidShape shape, GetSizeDelegate getSize, GetDescriptorDelegate getDescriptor)
        {
            _shape = shape;
            _getSize = getSize;
            _getDescriptor = getDescriptor;
            _depSize = new Dependent(UpdateSize);
            _depDescriptor = new Dependent(UpdateDescriptor);
            _depImage = new Dependent(UpdateImage);
        }

        public Size Size
        {
            get { _depSize.OnGet(); return _size; }
        }

        public SolidDescriptor Descriptor
        {
            get { _depDescriptor.OnGet(); return _descriptor; }
        }

        public SolidShape Shape
        {
            get { return _shape; }
        }

        public Bitmap Image
        {
            get
            {
                _depImage.OnGet();
                return _image;
            }
        }

        private void UpdateSize()
        {
            _size = _getSize();
        }

        private void UpdateDescriptor()
        {
            _descriptor = _getDescriptor();
        }

        private void UpdateImage()
        {
            _depDescriptor.OnGet();
            _depSize.OnGet();

            if (_size != _previousSize || _descriptor != _previousDescriptor)
            {
                _previousSize = _size;
                _previousDescriptor = _descriptor;

                // Release the previous image.
                if (_image != null)
                {
                    _image.Dispose();
                    Marshal.FreeCoTaskMem(_imageMemory);
                    _image = null;
                }

                // Short circuit.
                if (_size.Width <= 0 || _size.Height <= 0)
                    return;

                try
                {
                    // Convert to floating point.
                    PointF bounds = new PointF((float)_size.Width, (float)_size.Height);
                    PointF3D light = new PointF3D(_descriptor.Direction.X, _descriptor.Direction.Y, 0.0f);
                    float length = light.Dot(light);
                    if (length < 1.0f)
                        light.Z = (float)Math.Sqrt(1.0f - length);
                    else
                        light.Z = 0.0f;
                    float depth = _descriptor.Thickness * (float)Math.Min(_size.Width, _size.Height);

                    // Create an image buffer.
                    int stride = _size.Width * 4;
                    Int32[] buffer = new Int32[_size.Width * _size.Height];

                    // Calculate the color of each pixel.
                    float dimension = (float)Math.Max(_size.Width, _size.Height);
                    for (int y = 0; y < _size.Height; ++y)
                    {
                        for (int x = 0; x < _size.Width; ++x)
                        {
                            PointF p = new PointF((float)x, (float)y);
                            PointF3D normal;
                            float z;
                            float alpha;

                            if (_shape.Normal(bounds, p, out normal, out z, out alpha) && normal.IsValid)
                            {
                                // Bend z into a sharper angle.
                                // h = bz z / ((1-k)z + k)
                                // dh/dz = bz k / ((1-k)z + k)^2
                                float k = 1.0f - 0.99f * _descriptor.Sharpness;
                                float d = (1.0f - k) * z + k;
                                float dhdz = k / (d * d) * depth;
                                float height = z / d * depth;

                                // Adjust the normal.
                                // dh/dpx = dh/dz dz/dpx
                                // dh/dpy = dh/dz dz/dpy
                                normal.X *= dhdz * normal.Z;
                                normal.Y *= dhdz * normal.Z;
                                normal.Normalize();

                                // Calculate incident ray.
                                PointF3D incident = new PointF3D(
                                    x - _size.Width / 2,
                                    y - _size.Height / 2,
                                    -dimension);
                                incident.Normalize();
                                float cosNI = incident.Dot(normal);

                                float intensity = 1.0f;
                                if (_descriptor.Transparent)
                                {
                                    // Calculate reflection and refraction using Snell's law.
                                    float sin2NT = _descriptor.Refraction * _descriptor.Refraction * (1.0f - cosNI * cosNI);
                                    float cosNT = (float)Math.Sqrt(1.0f - sin2NT);
                                    float sinNT = (float)Math.Sqrt(sin2NT);
                                    PointF3D parallel = normal.Times(-1.0f * cosNI).Plus(incident);
                                    parallel.Normalize();
                                    PointF3D refraction = parallel.Times(sinNT).Plus(normal.Times(cosNT)).Times(-1.0f);
                                    refraction.Z = -refraction.Z;

                                    // Calculate diffuse illumination through refraction.
                                    intensity = refraction.Dot(light) * (1.0f - _descriptor.Ambience) + _descriptor.Ambience;
                                }
                                else
                                {
                                    // Calculate diffuse illumination.
                                    intensity = normal.Dot(light) * (1.0f - _descriptor.Ambience) + _descriptor.Ambience;
                                }

                                if (intensity < _descriptor.Ambience)
                                    intensity = _descriptor.Ambience;
                                if (intensity > 1.0f)
                                    intensity = 1.0f;

                                HLSColor color = new HLSColor(_descriptor.Color);
                                color.L = color.L * intensity;

                                // Calculate specular reflection.
                                if (_descriptor.Aperature > 0.0f)
                                {
                                    PointF3D reflection = normal.Times(-2.0f * cosNI).Plus(incident);
                                    intensity = 1.0f - (1.0f - reflection.Dot(light)) / _descriptor.Aperature;
                                    if (intensity > 0.0f)
                                    {
                                        color.L = color.L * (1.0f - intensity) + intensity;
                                    }
                                }
                                Color rgb = color.RGB;
                                if (alpha < 1.0f)
                                    rgb = Color.FromArgb((int)(255.0f * alpha), rgb);
                                buffer[y * _size.Width + x] = rgb.ToArgb();
                            }
                            else
                            {
                                buffer[y * _size.Width + x] = Color.Transparent.ToArgb();
                            }
                        }
                    }

                    _imageMemory = Marshal.AllocCoTaskMem(buffer.Length * 4);
                    Marshal.Copy(buffer, 0, _imageMemory, buffer.Length);
                    _image = new Bitmap(_size.Width, _size.Height, _size.Width * 4, PixelFormat.Format32bppArgb, _imageMemory);
                }
                catch (Exception x)
                {
                    Debug.WriteLine(string.Format("Error while updating solid cache. {0}", x.ToString()));
                }
            }
        }

        public void Dispose()
        {
            _depImage.Dispose();
            if (_image != null)
            {
                _image.Dispose();
                Marshal.FreeCoTaskMem(_imageMemory);
            }
        }
    }
}
