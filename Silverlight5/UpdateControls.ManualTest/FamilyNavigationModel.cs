using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class FamilyNavigationModel
    {
        private Independent<FamilyMember> _selectedMember = new Independent<FamilyMember>();

        public FamilyMember SelectedMember
        {
            get { return _selectedMember; }
            set { _selectedMember.Value = value; }
        }
    }
}
