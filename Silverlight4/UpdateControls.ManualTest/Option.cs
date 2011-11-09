using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class Option
    {
        private string _name;
        private string _group;
        private Independent<bool> _selected = new Independent<bool>();

        public Option(string group, string name)
        {
            _group = group;
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Group
        {
            get { return _group; }
        }

        public bool Selected
        {
            get { return _selected; }
            set { _selected.Value = value; }
        }

        public override bool Equals(object obj)
        {
            return _name.Equals(((Option)obj)._name) && _group.Equals(((Option)obj)._group);
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode() * 37 + _group.GetHashCode();
        }
    }
}
