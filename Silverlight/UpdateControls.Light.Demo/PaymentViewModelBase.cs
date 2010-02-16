using System;
using System.Collections.Generic;
using System.Linq;

namespace UpdateControls.Light.Demo
{
	public class PaymentViewModelBase
	{
		private Payment _payment;

		public PaymentViewModelBase(Payment payment)
		{
			_payment = payment;
		}

		public Payment Payment
		{
			get { return _payment; }
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
					(_payment.PaidInvoices.Any() ?
						_payment.PaidInvoices
							.Select(i => i.Number)
							.Aggregate((invoices, invoice) => invoices + ", " + invoice) :
						string.Empty);
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
	}
}
