Public Class Customer
    Dim _name As String
    Dim _invoices As List(Of Invoice)

    Dim _indName As New Independent
    Dim _indInvoices As New Independent

    Public Property Name() As String
        Get
            _indName.OnGet()
            Return _name
        End Get
        Set(ByVal value As String)
            _indName.OnSet()
            _name = value
        End Set
    End Property

    Public ReadOnly Property Invoices() As IEnumerable(Of Invoice)
        Get
            _indInvoices.OnGet()
            Return _invoices
        End Get
    End Property

    Public Function NewInvoice(ByVal number As String) As Invoice
        _indInvoices.OnSet()
        Dim invoice As New Invoice(number)
        _invoices.Add(invoice)
        Return invoice
    End Function
End Class
