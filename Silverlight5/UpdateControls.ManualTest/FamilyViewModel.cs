using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.XAML;

namespace UpdateControls.ManualTest
{
    public class FamilyViewModel
    {
        private readonly Family _family;
        private readonly FamilyNavigationModel _navigation;

        public FamilyViewModel(Family family, FamilyNavigationModel navigation)
        {
            _family = family;
            _navigation = navigation;
        }

        public string LastName
        {
            get { return _family.LastName; }
            set { _family.LastName = value; }
        }

        public string Title
        {
            get
            {
                return String.IsNullOrEmpty(_family.LastName)
                    ? "Your family"
                    : String.Format("The {0} family", _family.LastName);
            }
        }

        public IEnumerable<FamilyMemberSummaryViewModel> Members
        {
            get
            {
                return
                    from member in _family.Members
                    select new FamilyMemberSummaryViewModel(member, _family);
            }
        }

        public FamilyMemberSummaryViewModel SelectedMember
        {
            get
            {
                return _navigation.SelectedMember == null
                    ? null
                    : new FamilyMemberSummaryViewModel(_navigation.SelectedMember, _family);
            }
            set
            {
            	_navigation.SelectedMember = value == null
                    ? null
                    : value.Member;
            }
        }

        public bool MemberIsSelected
        {
            get { return _navigation.SelectedMember != null; }
        }

        public FamilyMemberDetailsViewModel SelectedMemberDetails
        {
            get
            {
                return _navigation.SelectedMember == null
                    ? null
                    : new FamilyMemberDetailsViewModel(_navigation.SelectedMember, _family);
            }
        }

        public ICommand NewMember
        {
            get
            {
                return MakeCommand
                    .Do(() =>
                    {
                        FamilyMember member = _family.NewMember();
                        _navigation.SelectedMember = member;
                    });
            }
        }

        public ICommand DeleteMember
        {
            get
            {
                return MakeCommand
                    .When(() => _navigation.SelectedMember != null)
                    .Do(() =>
                    {
                        _family.DeleteMember(_navigation.SelectedMember);
                        _navigation.SelectedMember = null;
                    });
            }
        }
    }
}
