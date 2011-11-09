using System.Collections.Generic;

namespace UpdateControls.ManualTest
{
    public class OptionHeading
    {
        private string _group;
        private string _heading;
        private List<Option> _options;

        public OptionHeading(string group, string heading, List<Option> options)
        {
            _group = group;
            _heading = heading;
            _options = options;
        }

        public string Group
        {
            get { return _group; }
        }

        public string Heading
        {
            get { return _heading; }
        }

        public IEnumerable<Option> Options
        {
            get { return _options; }
        }

        public override bool Equals(object obj)
        {
            return _group.Equals(((OptionHeading)obj)._group) && _heading.Equals(((OptionHeading)obj)._heading);
        }

        public override int GetHashCode()
        {
            return _group.GetHashCode() * 37 + _heading.GetHashCode();
        }
    }
}
