using System.Collections.Generic;
using UpdateControls.Collections;
using UpdateControls.Fields;

namespace UpdateControls.ManualTest
{
    public class Family
    {
        private Independent<string> _lastName = new Independent<string>();
        private IndependentList<FamilyMember> _members = new IndependentList<FamilyMember>();

        public string LastName
        {
            get { return _lastName; }
            set { _lastName.Value = value; }
        }

        public IEnumerable<FamilyMember> Members
        {
            get { return _members; }
        }

        public FamilyMember NewMember()
        {
            FamilyMember newMember = new FamilyMember();
            _members.Add(newMember);
            return newMember;
        }

        public void DeleteMember(FamilyMember member)
        {
            _members.Remove(member);
        }
    }
}
