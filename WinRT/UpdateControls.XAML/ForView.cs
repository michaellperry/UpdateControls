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
            // Ensure that AffectedSet has the ability to run delegates
            // on the UI thread.
            if (_mainDispatcher == null)
            {
                _mainDispatcher = Window.Current.Dispatcher;
            }
            AffectedSet.Initialize(RunOnUIThread);
        }

        public static object Wrap(object viewModel)
        {
            Initialize();
            if (viewModel == null)
                return null;
            return (IDependentObject)Activator.CreateInstance(
                typeof(DependentObject<>).MakeGenericType(viewModel.GetType()),
                viewModel);
        }

        public static TWrappedObjectType Unwrap<TWrappedObjectType>(object dataContext)
            where TWrappedObjectType : class
        {
            IDependentObject wrapper = dataContext as IDependentObject;
            return
                wrapper == null
                    ? default(TWrappedObjectType)
                    : wrapper.GetWrappedObject() as TWrappedObjectType;
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
