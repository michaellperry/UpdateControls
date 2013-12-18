using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateControls.Test
{
    public class GenderOption
    {
        private readonly GenderEnum _gender;

        public GenderOption(GenderEnum gender)
        {
            _gender = gender;
        }

        public GenderEnum Gender
        {
            get { return _gender; }
        }

        public override string ToString()
        {
            return _gender.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            GenderOption that = obj as GenderOption;
            if (that == null)
                return false;
            return Object.Equals(this._gender, that._gender);
        }

        public override int GetHashCode()
        {
            return _gender.GetHashCode();
        }
    }
}
