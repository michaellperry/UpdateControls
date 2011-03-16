using System;
using System.Collections.Generic;
using UpdateControls.Collections;

namespace UpdateControls.Light.Demo
{
    public class Payment
    {
        private Customer _customer;
        private IndependentList<Invoice> _paidInvoices = new IndependentList<Invoice>();

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
            get { return _paidInvoices; }
        }

        public void AddPaidInvoice(Invoice invoice)
        {
            _paidInvoices.Add(invoice);
        }

        public void RemovePaidInvoice(Invoice invoice)
        {
            _paidInvoices.Remove(invoice);
        }
    }
}
