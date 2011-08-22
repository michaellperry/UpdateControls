using System;

namespace UpdateControls.ManualTest
{
    public class FamilyViewModel
    {
        private Family _family;

        public FamilyViewModel(Family family)
        {
            _family = family;
        }

        public string LastName
        {
            get { return _family.LastName; }
            set { _family.LastName = value; }
        }

        public string Title
        {
            get
            {
                return String.IsNullOrEmpty(_family.LastName)
                    ? "Your family"
                    : String.Format("The {0} family", _family.LastName);
            }
        }
    }
}
