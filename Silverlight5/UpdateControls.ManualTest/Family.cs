using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class Family
    {
        private Independent<string> _lastName = new Independent<string>();

        public string LastName
        {
            get { return _lastName; }
            set { _lastName.Value = value; }
        }
    }
}
