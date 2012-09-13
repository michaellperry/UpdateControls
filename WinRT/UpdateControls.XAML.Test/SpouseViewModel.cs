using System;

namespace UpdateControls.XAML.Test
{
    public class SpouseViewModel : ViewModelBase, ISpouseViewModel
    {
        private Person _spouse;

        private SpouseViewModel(Person spouse)
        {
            _spouse = spouse;
        }

		public Person Spouse
		{
			get { return _spouse; }
		}

		public string FullName
        {
            get { return Get(() => _spouse == null ? "unmarried" : _spouse.FullName); }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            SpouseViewModel that = obj as SpouseViewModel;
            if (that == null)
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
    }
}
