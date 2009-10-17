// Patrick Cauldwell
// http://www.cauldwell.net/patrick/blog/MVVMBindingToCommandsInSilverlight.aspx

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace UpdateControls.Light.Demo
{
    public class ButtonService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Button _button;
        private ICommand _command;

        public ButtonService(Button button, ICommand command)
        {
            _button = button;
            _command = command;
        }

        public void OnClick(object sender, RoutedEventArgs arg)
        {
            _command.Execute(Parameter);
        }

        public bool IsEnabled
        {
            get { return _command.CanExecute(Parameter); }
        }

        public void CanExecuteChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("IsEnabled"));
        }

        private object Parameter
        {
            get { return _button.GetValue(CommandParameterProperty); }
        }

        #region CommandParameter

        /// <summary>
        /// CommandParameter Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(string), typeof(ButtonService),
                new PropertyMetadata(OnCommandParameterChanged));

        /// <summary>
        /// Gets the CommandParameter property.
        /// </summary>
        public static string GetCommandParameter(DependencyObject d)
        {
            return (string)d.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the CommandParameter property.
        /// </summary>
        public static void SetCommandParameter(DependencyObject d, string value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Handles changes to the CommandParameter property.
        /// </summary>
        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion
        
        #region Command

        /// <summary>
        /// Command Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ButtonService),
                new PropertyMetadata(OnCommandChanged));

        /// <summary>
        /// Gets the Command property.
        /// </summary>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the Command property.
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button)
            {
                // Handle the click event.
                Button b = d as Button;
                ICommand c = e.NewValue as ICommand;
                ButtonService buttonService = new ButtonService(b, c);
                b.Click += buttonService.OnClick;

                // Handle the CanExecuteChanged event.
                c.CanExecuteChanged += buttonService.CanExecuteChanged;

                // Bind the IsEnabled event.
                Binding binding = new Binding("IsEnabled");
                binding.Source = buttonService;
                b.SetBinding(Button.IsEnabledProperty, binding);
            }
        }

        #endregion
    }
}
