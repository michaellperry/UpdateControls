using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UpdateControls.Collections;
using UpdateControls.Fields;

namespace $rootnamespace$.Models
{
    public class Item
    {
        private Independent<string> _name = new Independent<string>();

        public string Name
        {
            get { return _name; }
            set { _name.Value = value; }
        }
    }
}
