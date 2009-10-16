using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.XAML.Test
{
    public class SpouseViewModel
    {
        private Person _spouse;

        private SpouseViewModel(Person spouse)
        {
            _spouse = spouse;
        }

        public string FullName
        {
            get { return _spouse == null ? "unmarried" : _spouse.FullName; }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            SpouseViewModel that = obj as SpouseViewModel;
            if (obj == null)
                return false;
            return object.Equals(this._spouse, that._spouse);
        }

        public override int GetHashCode()
        {
            return _spouse == null ? 0 : _spouse.GetHashCode();
        }

        public static SpouseViewModel Wrap(Person spouse)
        {
            return new SpouseViewModel(spouse);
        }

        public static Person Unwrap(SpouseViewModel viewModel)
        {
            if (viewModel == null)
                return null;
            else
                return viewModel._spouse;
        }
    }
}
