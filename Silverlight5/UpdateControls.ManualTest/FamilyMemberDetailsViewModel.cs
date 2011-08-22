using System;

namespace UpdateControls.ManualTest
{
    public class FamilyMemberDetailsViewModel
    {
        private readonly FamilyMember _member;

        public FamilyMemberDetailsViewModel(FamilyMember member)
        {
            _member = member;
        }

        public string FirstName
        {
            get { return _member.FirstName; }
            set { _member.FirstName = value; }
        }
    }
}
