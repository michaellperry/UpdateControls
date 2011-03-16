using UpdateControls.Fields;

namespace UpdateControls.Light.UnitTest
{
    public class Child
    {
        private Independent<Parent> _mom = new Independent<Parent>();
        private Independent<Parent> _dad = new Independent<Parent>();

        public Parent Mom
        {
            get { return _mom; }
            set { _mom.Value = value; }
        }

        public Parent Dad
        {
            get { return _dad; }
            set { _dad.Value = value; }
        }
    }
}
