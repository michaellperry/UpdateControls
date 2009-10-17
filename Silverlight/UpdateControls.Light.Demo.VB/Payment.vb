Public Class Payment
    Public Sub New()
        Throw New NotImplementedException()
    End Sub
    Public ReadOnly Property PaidInvoices() As System.Collections.Generic.IEnumerable(Of Invoice)
        Get
            Throw New NotImplementedException()
        End Get
    End Property
    Public ReadOnly Property Customer() As Customer
        Get
            Throw New NotImplementedException()
        End Get
    End Property
End Class
