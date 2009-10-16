/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2008 Mallard Software Designs
 * Licensed under LGPL
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UpdateControls.Test
{
    [Serializable, CLSCompliant(true)]
    public class Person
    {
        public class NameException : ApplicationException
        {
            public NameException(string message) : base(message) { }
        }
        public class PhoneException : ApplicationException
        {
            public PhoneException(string message) : base(message) { }
        }

        private string _name = string.Empty;
        private string _phone = string.Empty;
        private bool _requirePhone = false;
        private Company _employer = null;

        private Independent _dynName = new Independent();
        private Independent _dynPhone = new Independent();
        private Independent _dynRequirePhone = new Independent();
        private Independent _dynEmployer = new Independent();

        private static Regex[] ValidPhone = new Regex[] {
            new Regex(@"(?'area'\d{3})\s*(?'exchange'\d{3})\s*(?'num'\d{4})", RegexOptions.Singleline),
            new Regex(@"(?'area'\d{3})\.(?'exchange'\d{3})\.(?'num'\d{4})", RegexOptions.Singleline),
            new Regex(@"\((?'area'\d{3})\)\s*(?'exchange'\d{3})-(?'num'\d{4})", RegexOptions.Singleline)
        };

        public string Name
        {
            get { _dynName.OnGet(); return _name; }
            set { _dynName.OnSet(); _name = value; }
        }

        public string Phone
        {
            get { _dynPhone.OnGet(); try { return ValidatePhone(); } catch (ApplicationException) { return _phone; } }
            set { _dynPhone.OnSet(); _phone = value; }
        }

        public bool RequirePhone
        {
            get { _dynRequirePhone.OnGet(); return _requirePhone; }
            set { _dynRequirePhone.OnSet(); _requirePhone = value; }
        }

        public Company Employer
        {
            get { _dynEmployer.OnGet(); return _employer; }
            set { _dynEmployer.OnSet(); _employer = value; }
        }

        public void Validate()
        {
            _dynName.OnGet();
            if (string.IsNullOrEmpty(_name))
                throw new NameException("Please enter a name.");
            _dynPhone.OnGet();
            ValidatePhone();
        }

        private string ValidatePhone()
        {
            foreach (Regex regex in ValidPhone)
            {
                Match match = regex.Match(_phone);
                if (match.Success)
                {
                    return string.Format("({0}) {1}-{2}",
                        match.Groups["area"].Value,
                        match.Groups["exchange"].Value,
                        match.Groups["num"].Value);
                }
            }
            throw new PhoneException("Please enter a valid phone number.");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
