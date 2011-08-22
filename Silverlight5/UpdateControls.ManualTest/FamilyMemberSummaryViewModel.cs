using System;

namespace UpdateControls.ManualTest
{
    public class FamilyMemberSummaryViewModel
    {
        private readonly FamilyMember _member;
        private readonly Family _family;

        public FamilyMemberSummaryViewModel(FamilyMember member, Family family)
        {
            _family = family;
            _member = member;
        }

        public FamilyMember Member
        {
            get { return _member; }
        }

        public string FullName
        {
            get
            {
                return String.IsNullOrEmpty(_member.FirstName)
                    ? "<New Member>"
                    : String.Format("{0} {1}", _member.FirstName, _family.LastName);
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            FamilyMemberSummaryViewModel that = obj as FamilyMemberSummaryViewModel;
            if (that == null)
                return false;
            return _member.Equals(that._member);
        }

        public override int GetHashCode()
        {
            return _member.GetHashCode();
        }
    }
}
