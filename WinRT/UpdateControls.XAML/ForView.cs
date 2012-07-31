using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.XAML
{
    public static class ForView
    {
        public static object Wrap(object viewModel)
        {
            return viewModel == null
                ? null
                : new Wrapper.DynamicDependentWrapper(viewModel);
        }
    }
}
