using System;
using System.Collections.Generic;
using UpdateControls.Collections;
using UpdateControls.Fields;

namespace UpdateControls.Light.Demo
{
    public class Customer
    {
        private Independent<string> _name = new Independent<string>();
        private IndependentList<Invoice> _invoices = new IndependentList<Invoice>();

        public string Name
        {
            get { return _name; }
            set { _name.Value = value; }
        }

        public IEnumerable<Invoice> Invoices
        {
            get { return _invoices; }
        }

        public Invoice NewInvoice(string number)
        {
            Invoice invoice = new Invoice(number);
            _invoices.Add(invoice);
            return invoice;
        }
    }
}
