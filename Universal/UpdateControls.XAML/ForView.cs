using System;
using UpdateControls.XAML.Wrapper;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UpdateControls.XAML
{
    public static class ForView
    {
        private static CoreDispatcher _mainDispatcher;

        public static void Initialize()
        {
            // Ensure that the UpdateScheduler has the ability to run delegates
            // on the UI thread.
            if (_mainDispatcher == null)
            {
                _mainDispatcher = Window.Current.Dispatcher;
            }
            UpdateScheduler.Initialize(RunOnUIThread);
        }

        public static object Wrap(object viewModel)
        {
            Initialize();
            if (viewModel == null)
                return null;
            return (IObjectInstance)Activator.CreateInstance(
                typeof(ObjectInstance<>).MakeGenericType(viewModel.GetType()),
                viewModel);
        }

        public static TWrappedObjectType Unwrap<TWrappedObjectType>(object dataContext)
            where TWrappedObjectType : class
        {
            IObjectInstance wrapper = dataContext as IObjectInstance;
            return
                wrapper == null
                    ? default(TWrappedObjectType)
                    : wrapper.WrappedObject as TWrappedObjectType;
        }

        private static async void RunOnUIThread(Action action)
        {
            if (_mainDispatcher != null)
            {
                await _mainDispatcher.RunAsync(
                    CoreDispatcherPriority.Low,
                    new DispatchedHandler(delegate
                    {
                        action();
                    }));
            }
        }
    }
}
