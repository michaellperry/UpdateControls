using System;
using System.Collections.Generic;

namespace UpdateControls.Light.Demo
{
    public class Customer
    {
        private string _name;
        private List<Invoice> _invoices = new List<Invoice>();

        private Independent _indName = new Independent();
        private Independent _indInvoices = new Independent();

        public string Name
        {
            get { _indName.OnGet(); return _name; }
            set { _indName.OnSet(); _name = value; }
        }

        public IEnumerable<Invoice> Invoices
        {
            get { _indInvoices.OnGet(); return _invoices; }
        }

        public Invoice NewInvoice(string number)
        {
            _indInvoices.OnSet();
            Invoice invoice = new Invoice(number);
            _invoices.Add(invoice);
            return invoice;
        }
    }
}
