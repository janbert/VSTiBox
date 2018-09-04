Imports System.Windows.Forms

Public Class ImportProgress

  Private Sub ImportProgress_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

  End Sub

  Public Sub UpdateProgress(ByVal message As String, ByVal progress As Integer)
    Label1.Text = message
    ProgressBar1.Increment(progress)
    Label1.Update()
  End Sub

End Class
