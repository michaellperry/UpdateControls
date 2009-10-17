using System;
using System.Collections.Generic;

namespace UpdateControls.Light.Demo
{
    public class Payment
    {
        private Customer _customer;
        private List<Invoice> _paidInvoices = new List<Invoice>();

        private Independent _indPaidInvoices = new Independent();

        public Payment(Customer customer)
        {
            _customer = customer;
        }

        public Customer Customer
        {
            get { return _customer; }
        }

        public IEnumerable<Invoice> PaidInvoices
        {
            get { _indPaidInvoices.OnGet(); return _paidInvoices; }
        }

        public void AddPaidInvoice(Invoice invoice)
        {
            _indPaidInvoices.OnSet();
            _paidInvoices.Add(invoice);
        }

        public void RemovePaidInvoice(Invoice invoice)
        {
            _indPaidInvoices.OnSet();
            _paidInvoices.Remove(invoice);
        }
    }
}
