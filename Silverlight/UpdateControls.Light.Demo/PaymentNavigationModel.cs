using System;

namespace UpdateControls.Light.Demo
{
    public class PaymentNavigationModel
    {
        private Invoice _selectedUnpaidInvoice;
        private Invoice _selectedPaidInvoice;

        private Independent _indSelectedUnpaidInvoice = new Independent();
        private Independent _indSelectedPaidInvoice = new Independent();

        public Invoice SelectedUnpaidInvoice
        {
            get { _indSelectedUnpaidInvoice.OnGet(); return _selectedUnpaidInvoice; }
            set { _indSelectedUnpaidInvoice.OnSet(); _selectedUnpaidInvoice = value; }
        }

        public Invoice SelectedPaidInvoice
        {
            get { _indSelectedPaidInvoice.OnGet(); return _selectedPaidInvoice; }
            set { _indSelectedPaidInvoice.OnSet(); _selectedPaidInvoice = value; }
        }
    }
}
