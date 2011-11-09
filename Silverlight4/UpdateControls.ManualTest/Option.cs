using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class Option
    {
        private string _name;
        private Independent<bool> _selected = new Independent<bool>();

        public Option(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected.Value = value; }
        }

        public override bool Equals(object obj)
        {
            return _name.Equals(((Option)obj)._name);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode();
        }
    }
}
