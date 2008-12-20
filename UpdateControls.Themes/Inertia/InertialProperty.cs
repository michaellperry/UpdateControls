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
using UpdateControls.Forms;
using System.Runtime.InteropServices;

namespace UpdateControls.Themes.Inertia
{
    public class InertialProperty : IDisposable
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetTickCount();

        // Constant
        private const float SPEED = 0.003F;
        private const float EPSILON = 1e-6F;

        // Definitive
        private GetFloatDelegate _getTargetValue;
        private GetBoolDelegate _getHasInertia;

        // Dynamic
        private long _ticks = 0;
        private Independent _dynTicks = new Independent();

        // Dependent
        private float _targetValue;
        private bool _hasInertia;
        private Dependent _depTargetValue;

        private bool _initialized = false;
        private long _targetTicks;
        private float _a;
        private float _c;
        private bool _atRest = true;
        private Dependent _depParameters;

        private float _value;
        private Dependent _depValue;

        public InertialProperty(GetFloatDelegate getTargetValue, GetBoolDelegate getHasInertia)
        {
            _getTargetValue = getTargetValue;
            _getHasInertia = getHasInertia;
            _depTargetValue = new Dependent(UpdateTargetValue);
            _depParameters = new Dependent(UpdateParameters);
            _depValue = new Dependent(UpdateValue);
        }

        private void UpdateTargetValue()
        {
            _targetValue = _getTargetValue();
            _hasInertia = _getHasInertia();
        }

        private void UpdateParameters()
        {
            // If we're currently at rest, start the timer.
            if (_atRest)
                _ticks = GetTickCount();

            _depTargetValue.OnGet();
            if (!_initialized)
            {
                // Start at the target value.
                _targetTicks = _ticks;
                _a = 1.0F;
                _c = _targetValue;
                _initialized = true;
            }
            else if (!_hasInertia)
            {
                // Fix at the target value.
                _targetTicks = _ticks;
                _a = 1.0F;
                _c = _targetValue;
            }
            else if (_c != _targetValue)
            {
                // Get the starting state using the previous parameters.
                float y0;
                float yp0;
                if (_ticks < _targetTicks)
                {
                    float x = (_ticks - _targetTicks) * SPEED;
                    y0 = _a * (2.0F * x + 3.0F) * x * x + _c;
                    yp0 = _a * (6.0F * x + 6.0F) * x;
                }
                else
                {
                    y0 = _c;
                    yp0 = 0.0F;
                }

                // Calculate the parameters of the curve based on the target value.
                float x0 = -1.0F;
                if (Math.Abs(yp0) > EPSILON)
                {
                    float t = 6.0F * (y0 - _targetValue) / yp0;         // Temporary
                    float d = (float)Math.Sqrt((t + 2.0F) * t + 9.0F);  // Square root of discriminant
                    float c = (t - 3.0F + d) / 4.0F;                    // Larger candidate root.

                    // x0 must be negative. If larger candidate is negative, use it. Otherwise, use smaller one.
                    x0 = (c < 0.0F) ? c : ((t - 3.0F - d) / 4.0F);
                }
                _a = (y0 - _targetValue) / ((2.0F * x0 + 3.0F) * x0 * x0);
                _c = _targetValue;
                _targetTicks = _ticks - (long)(x0 / SPEED);
            }

            _atRest = false;
        }

        private void UpdateValue()
        {
            _depParameters.OnGet();
            if (!_atRest)
            {
                // Calculate the current value based on the current time and the parameters of the curve.
                _dynTicks.OnGet();
                if (_ticks >= _targetTicks)
                {
                    // We've reached the target. Stop moving.
                    _atRest = true;
                    _value = _c;
                }
                else
                {
                    // Determine the current position based on how much time is left.
                    float x = (_ticks - _targetTicks) * SPEED;
                    _value = _a * (2.0F * x + 3.0F) * x * x + _c;
                }
            }
        }

        public bool OnTimer(long ticks)
        {
            _ticks = ticks;
            _depParameters.OnGet();
            if (!_atRest)
            {
                _dynTicks.OnSet();
                return true;
            }
            else
                return false;
        }

        public float Value
        {
            get
            {
                _depValue.OnGet();
                return _value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _depParameters.Dispose();
            _depTargetValue.Dispose();
            _depValue.Dispose();
        }

        #endregion
    }
}
