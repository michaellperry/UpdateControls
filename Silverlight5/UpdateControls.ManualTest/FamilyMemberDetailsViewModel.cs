using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.ManualTest
{
    public class FamilyMemberDetailsViewModel
    {
        private readonly FamilyMember _member;
        private readonly Family _family;

        public FamilyMemberDetailsViewModel(FamilyMember member, Family family)
        {
            _member = member;
            _family = family;
        }

        public string FirstName
        {
            get { return _member.FirstName; }
            set { _member.FirstName = value; }
        }

        public IEnumerable<string> FirstNameOptions
        {
            get
            {
                return
                    from otherMember in _family.Members
                    where otherMember != _member
                    orderby otherMember.FirstName
                    select otherMember.FirstName;
            }
        }
    }
}
