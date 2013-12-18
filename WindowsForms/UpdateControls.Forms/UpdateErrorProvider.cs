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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace UpdateControls.Forms
{
    /// <summary>
    /// An error provider that automatically updates its properties.
    /// </summary>
    [Description("An error provider that automatically updates its properties."),
   ToolboxBitmap(typeof(UpdateErrorProvider), "ToolboxImages.UpdateErrorProvider.png"),
    DefaultProperty("Name"),
    DefaultEvent("GetErrorText")]
    [LicenseProviderAttribute(typeof(LicFileLicenseProvider))]
    [ProvideProperty("GetError", typeof(Control))]
    public partial class UpdateErrorProvider : Component, IExtenderProvider
    {
        private class ControlValidater
        {
            // Definitive.
            private ErrorProvider _errorProvider;
            private Control _control;
            private GetStringDelegate _getError;

            // Dependent error text.
            private Dependent _depErrorText;
            private string _errorText;

            public ControlValidater(
                ErrorProvider errorProvider,
                Control control,
                GetStringDelegate getError)
            {
                _errorProvider = errorProvider;
                _control = control;
                _getError = getError;

                _depErrorText = new Dependent(UpdateControlError);
            }

            public void Dispose()
            {
                _depErrorText.Dispose();
            }

            // Put the error text into the error provider.
            public void SetError()
            {
                _depErrorText.OnGet();
                _errorProvider.SetError(_control, _errorText);
            }

            // Update the error text.
            private void UpdateControlError()
            {
                if (_control is IEnabledControl && !((IEnabledControl)_control).Enabled)
                    _errorText = string.Empty;
                else
                {
                    string errorText = string.Empty;
                    if (_control is IErrorControl)
                    {
                        IErrorControl errorControl = (IErrorControl)_control;
                        errorText = errorControl.GetError();
                    }
                    if (string.IsNullOrEmpty(errorText))
                        errorText = _getError();
                    _errorText = errorText;
                }
            }
        }

        // The control that this error provider belongs to.
        private ContainerControl _containerControl;

        // Map of controls to the names of their GetError methods.
        private IDictionary<Control, string> _getErrorMethod = new Dictionary<Control, string>();

        // Turn validation on and off.
        private Independent _dynValidation = new Independent();
        private bool _validation = true;

        // Visible sentry to update error text.
        private Dependent _visErrorText;

        // Intermediate sentries for each control.
        private IDictionary<Control, ControlValidater> _controlSentry = new Dictionary<Control, ControlValidater>();

        public UpdateErrorProvider()
        {
            _visErrorText = new Dependent(UpdateError);
            InitializeComponent();
            Application.Idle += new EventHandler(Application_Idle);
        }

        public UpdateErrorProvider(IContainer container)
        {
            _visErrorText = new Dependent(UpdateError);
            container.Add(this);

            InitializeComponent();
            Application.Idle += new EventHandler(Application_Idle);
        }

        public bool CanExtend(object extendee)
        {
            return extendee is Control;
        }

        /// <summary>
        /// Get the name of the GetError method. This method must take no parameters
        /// and return a string. An empty string indicates no error.
        /// </summary>
        /// <param name="control">The control associated with the error produced.</param>
        /// <returns></returns>
        [DefaultValue("")]
        [Description("The method that returns the error text for this control. " +
            "This method must take no parameters and return a string. An empty " +
            "string indicates no error."), Category("Update")]
        public string GetGetError(Control control)
        {
            string error;
            if (_getErrorMethod.TryGetValue(control, out error))
                return error;
            else
                return string.Empty;
        }

        /// <summary>
        /// Set the name of the GetError method. This method must take no parameters
        /// and return a string. An empty string indicates no error.
        /// </summary>
        /// <param name="control">The control associated with the error produced.</param>
        /// <param name="method">The name of the method that produces an error message.</param>
        public void SetGetError(Control control, string method)
        {
            if (string.IsNullOrEmpty(method))
                _getErrorMethod.Remove(control);
            else
                _getErrorMethod[control] = method;
        }

        // Update the control sentries.
        private void UpdateError()
        {
            if (!this.DesignMode)
            {
                _dynValidation.OnGet();
                if (_validation && _containerControl != null)
                {
                    foreach (Control control in AllControls(_containerControl))
                    {
                        Control thisControl = control;
                        ControlValidater validater;
                        if (_controlSentry.TryGetValue(thisControl, out validater))
                        {
                            validater.SetError();
                        }
                        else
                        {
                            // Create a sentry if the control has an error method, or is an error control.
                            string getErrorMethodName;
                            bool hasErrorMethod = _getErrorMethod.TryGetValue(thisControl, out getErrorMethodName);
                            if (hasErrorMethod || thisControl is IErrorControl)
                            {
                                if (!hasErrorMethod)
                                {
                                    // Create a control validater with no error method.
                                    ControlValidater controlValidater = new ControlValidater(
                                        errorProvider,
                                        thisControl,
                                        delegate()
                                        {
                                            return string.Empty;
                                        });
                                    _controlSentry[thisControl] = controlValidater;
                                    controlValidater.SetError();
                                }
                                else
                                {
                                    // Find the error method.
                                    MethodInfo getErrorMethod = _containerControl.GetType().GetMethod(
                                        getErrorMethodName,
                                        new Type[0]);
                                    if (getErrorMethod == null)
                                        errorProvider.SetError(thisControl, string.Format(
                                            "The method \"string {0}.{1}()\" was not found.",
                                            _containerControl.GetType().FullName,
                                            getErrorMethodName));
                                    else
                                    {
                                        // Create a control validater.
                                        ControlValidater controlValidater = new ControlValidater(
                                            errorProvider,
                                            thisControl,
                                            delegate()
                                            {
                                                return getErrorMethod.Invoke(_containerControl, new object[0]).ToString();
                                            });
                                        _controlSentry[thisControl] = controlValidater;
                                        controlValidater.SetError();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorProvider.Clear();
                }
            }
        }

        private IEnumerable<Control> AllControls(Control containerControl)
        {
            foreach (Control control in containerControl.Controls)
            {
                yield return control;
                foreach (Control subControl in AllControls(control))
                    yield return subControl;
            }
        }

        /// <summary>
        /// Turn on validation and determine whether any errors have been found.
        /// </summary>
        /// <returns>True if there are no errors.</returns>
        public bool Validate()
        {
            if (_containerControl != null)
            {
                // Turn on validation.
                _dynValidation.OnSet();
                _validation = true;

                // Update the error text.
                _visErrorText.OnGet();

                // Return false if any errors have been found.
                foreach (Control control in AllControls(_containerControl))
                {
                    if (!string.IsNullOrEmpty(errorProvider.GetError(control)))
                        return false;
                }
            }

            // All controls have been validated.
            return true;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            // Update all dependent sentries.
            _visErrorText.OnGet();
        }

        /// <summary>
        /// Gets or sets the reference to the object that implents the GetError methods.
        /// </summary>
        [Description("The object that implements the GetError methods. Specify the name of the" +
            " method in the GetError property of each control."), Category("Update")]
        public ContainerControl ContainerControl
        {
            get { return _containerControl; }
            set { _containerControl = value; }
        }

        /// <summary>
        /// Turns validation on or off. When validation is on, error icons appear and
        /// disapear as data changes. When validation is off, error icons do not appear.
        /// </summary>
        [Description("Turns validation on or off. When validation is on, error icons appear and " +
            "disapear as data changes. When validation is off, error icons do not appear."), Category("Update"), DefaultValue(true)]
        public bool Validation
        {
            get { _dynValidation.OnGet(); return _validation; }
            set { _dynValidation.OnSet(); _validation = value; }
        }

        /// <summary>
        /// Gets or sets the System.Drawing.Icon that is displayed next to a control when
        /// an error discription string has been set for the control.
        /// </summary>
        [Description("The icon used to indicate an error."), Category("Appearance")]
        public Icon Icon
        {
            get { return errorProvider.Icon; }
            set { errorProvider.Icon = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the component is used in a locale
        /// that supports right-to-left fonts.
        /// </summary>
        [Description("Indicates whether the component should draw right-to-left for RTL languages."), Category("Appearance"), DefaultValue(false)]
        public bool RightToLeft
        {
            get { return errorProvider.RightToLeft; }
            set { errorProvider.RightToLeft = value; }
        }

        /// <summary>
        /// Gets or sets the rate at which the error icon flashes.
        /// </summary>
        [Description("The rate in milliseconds at which the error icon blinks."), Category("Behavior"), DefaultValue(250)]
        public int BlinkRate
        {
            get { return errorProvider.BlinkRate; }
            set { errorProvider.BlinkRate = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating when the error icon flashes.
        /// </summary>
        [Description("Controls whether the error icon blinks when an error is set."), Category("Behavior"), DefaultValue(ErrorBlinkStyle.BlinkIfDifferentError)]
        public ErrorBlinkStyle BlinkStyle
        {
            get { return errorProvider.BlinkStyle; }
            set { errorProvider.BlinkStyle = value; }
        }
    }
}
