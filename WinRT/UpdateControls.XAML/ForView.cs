using System;
using UpdateControls.XAML.Wrapper;

namespace UpdateControls.XAML
{
    public static class ForView
    {
        public static object Wrap(object viewModel)
        {
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
    }
}
