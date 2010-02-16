using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using UpdateControls.XAML;

namespace UpdateControls.Light.Demo
{
    public class PaymentViewModel : PaymentViewModelBase
	{
		private PaymentNavigationModel _navigation;

        public PaymentViewModel(Payment payment, PaymentNavigationModel navigation)
			: base(payment)
        {
            _navigation = navigation;
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
                        Payment.AddPaidInvoice(_navigation.SelectedUnpaidInvoice);
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
                        Payment.RemovePaidInvoice(_navigation.SelectedPaidInvoice);
                        _navigation.SelectedUnpaidInvoice = _navigation.SelectedPaidInvoice;
                        _navigation.SelectedPaidInvoice = null;
                    });
            }
        }
    }
}
