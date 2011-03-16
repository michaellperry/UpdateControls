using System;
using UpdateControls.Fields;

namespace UpdateControls.Light.Demo
{
    public class PaymentNavigationModel
    {
        private Independent<Invoice> _selectedUnpaidInvoice = new Independent<Invoice>();
        private Independent<Invoice> _selectedPaidInvoice = new Independent<Invoice>();

        public Invoice SelectedUnpaidInvoice
        {
            get { return _selectedUnpaidInvoice; }
            set { _selectedUnpaidInvoice.Value = value; }
        }

        public Invoice SelectedPaidInvoice
        {
            get { return _selectedPaidInvoice; }
            set { _selectedPaidInvoice.Value = value; }
        }
    }
}
