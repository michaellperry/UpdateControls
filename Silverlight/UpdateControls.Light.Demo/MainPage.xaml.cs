using System.Windows.Controls;
using UpdateControls.XAML;

namespace UpdateControls.Light.Demo
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            Customer customer = new Customer();
            // Load the customer from the web service.
            customer.Name = "Primatech Paper";
            Invoice invoice1 = customer.NewInvoice("1001");
            Invoice invoice2 = customer.NewInvoice("1002");
            Invoice invoice3 = customer.NewInvoice("1003");

            Payment payment = new Payment(customer);
            // Load the payment from the web service.
            payment.AddPaidInvoice(invoice1);

            DataContext = ForView.Wrap(
                new PaymentViewModel(
                    payment,
                    new PaymentNavigationModel()));

        }
    }
}
