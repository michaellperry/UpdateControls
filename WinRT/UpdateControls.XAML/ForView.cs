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
    }
}
