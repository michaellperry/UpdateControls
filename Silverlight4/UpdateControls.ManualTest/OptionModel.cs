using System.Collections.Generic;
using System.Linq;
using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class OptionModel
    {
        private List<OptionHeading> _headings = new List<OptionHeading>
        {
            new OptionHeading("Group 1", "Heading I", new List<Option>
            {
                new Option("Option A"),
                new Option("Option B"),
                new Option("Option C")
            }),
            new OptionHeading("Group 1", "Heading II", new List<Option>
            {
                new Option("Option D"),
                new Option("Option E"),
                new Option("Option F")
            }),
            new OptionHeading("Group 2", "Heading I", new List<Option>
            {
                new Option("Option G"),
                new Option("Option H"),
                new Option("Option I")
            }),
            new OptionHeading("Group 2", "Heading II", new List<Option>
            {
                new Option("Option J"),
                new Option("Option K"),
                new Option("Option L")
            }),
        };
        private Independent<string> _selectedGroup = new Independent<string>();

        public IEnumerable<string> Groups
        {
            get
            {
                yield return "Group 1";
                yield return "Group 2";
            }
        }

        public string SelectedGroup
        {
            get { return _selectedGroup; }
            set { _selectedGroup.Value = value; }
        }

        public IEnumerable<OptionHeading> Headings
        {
            get
            {
                return
                    from h in _headings
                    where h.Group == SelectedGroup
                    select h;
            }
        }
    }
}
