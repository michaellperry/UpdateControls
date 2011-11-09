using System.Collections.Generic;
using System.Linq;
using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class OptionModel
    {
        private List<Option> _options = new List<Option>
        {
            new Option("Group 1", "Option A"),
            new Option("Group 1", "Option B"),
            new Option("Group 1", "Option C"),
            new Option("Group 2", "Option A"),
            new Option("Group 2", "Option B"),
            new Option("Group 2", "Option C"),
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

        public IEnumerable<Option> Options
        {
            get
            {
                return
                    from o in _options
                    where o.Group == SelectedGroup
                    select o;
            }
        }
    }
}
