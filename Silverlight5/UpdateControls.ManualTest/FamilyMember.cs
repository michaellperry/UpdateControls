using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class FamilyMember
    {
        private Independent<string> _firstName = new Independent<string>();

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName.Value = value; }
        }
    }
}
