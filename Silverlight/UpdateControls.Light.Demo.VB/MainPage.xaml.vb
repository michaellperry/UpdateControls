Partial Public Class MainPage
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        Dim payment As Payment = New Payment
        DataContext = ForView.Wrap( _
            New PaymentViewModel(payment, _
                New PaymentNavigationModel))
    End Sub

End Class