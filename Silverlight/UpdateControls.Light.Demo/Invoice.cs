using System;

namespace UpdateControls.Light.Demo
{
    public class Invoice
    {
        private string _number;

        public Invoice(string number)
        {
            _number = number;
        }

        public string Number
        {
            get { return _number; }
        }
    }
}
