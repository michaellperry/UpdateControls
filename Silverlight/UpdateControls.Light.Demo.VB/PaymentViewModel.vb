Public Class PaymentViewModel
    Dim _payment As Payment
    Dim _navigation As PaymentNavigationModel

    Public Sub New(ByVal payment As Payment, ByVal navigation As PaymentNavigationModel)
        _payment = payment
        _navigation = navigation
    End Sub

    Public ReadOnly Property PaidInvoices() As IEnumerable(Of Invoice)
        Get
            Return _payment.PaidInvoices
        End Get
    End Property

    Public ReadOnly Property UnpaidInvoices() As IEnumerable(Of Invoice)
        Get
            Return _payment.Customer.Invoices.Except(_payment.PaidInvoices)
        End Get
    End Property
End Class
