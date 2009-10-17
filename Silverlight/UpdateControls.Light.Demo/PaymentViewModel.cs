using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.XAML;

namespace UpdateControls.Light.Demo
{
    public class PaymentViewModel
    {
        private Payment _payment;
        private PaymentNavigationModel _navigation;

        public PaymentViewModel(Payment payment, PaymentNavigationModel navigation)
        {
            _payment = payment;
            _navigation = navigation;
        }

        public string CustomerName
        {
            get { return _payment.Customer.Name; }
            set { _payment.Customer.Name = value; }
        }

        public string Summary
        {
            get
            {
                return _payment.Customer.Name + ": " +
                    _payment.PaidInvoices
                        .Select(i => i.Number)
                        .Aggregate((invoices, invoice) => invoices + ", " + invoice);
            }
        }

        public IEnumerable<Invoice> PaidInvoices
        {
            get { return _payment.PaidInvoices; }
        }

        public IEnumerable<Invoice> UnpaidInvoices
        {
            get { return _payment.Customer.Invoices.Except(_payment.PaidInvoices); }
        }

        public Invoice SelectedPaidInvoice
        {
            get { return _navigation.SelectedPaidInvoice; }
            set { _navigation.SelectedPaidInvoice = value; }
        }

        public Invoice SelectedUnpaidInvoice
        {
            get { return _navigation.SelectedUnpaidInvoice; }
            set { _navigation.SelectedUnpaidInvoice = value; }
        }

        public ICommand AddPaidInvoices
        {
            get
            {
                return MakeCommand
                    .When(() => _navigation.SelectedUnpaidInvoice != null)
                    .Do(() =>
                    {
                        _payment.AddPaidInvoice(_navigation.SelectedUnpaidInvoice);
                        _navigation.SelectedPaidInvoice = _navigation.SelectedUnpaidInvoice;
                        _navigation.SelectedUnpaidInvoice = null;
                    });
            }
        }

        public ICommand RemovePaidInvoices
        {
            get
            {
                return MakeCommand
                    .When(() => _navigation.SelectedPaidInvoice != null)
                    .Do(() =>
                    {
                        _payment.RemovePaidInvoice(_navigation.SelectedPaidInvoice);
                        _navigation.SelectedUnpaidInvoice = _navigation.SelectedPaidInvoice;
                        _navigation.SelectedPaidInvoice = null;
                    });
            }
        }
    }
}
