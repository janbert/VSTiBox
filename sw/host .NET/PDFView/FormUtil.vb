Imports System.Drawing
Imports System.Windows.Forms

Public Class FormUtil

  Public Shared Function GetFormImage(ByRef myForm As Form) As Bitmap
    Dim g As Graphics = myForm.CreateGraphics()
    Dim s As Size = myForm.Size
    GetFormImage = New Bitmap(s.Width, s.Height, g)
    Dim mg As Graphics = Graphics.FromImage(GetFormImage)
    Dim dc1 As IntPtr = g.GetHdc
    Dim dc2 As IntPtr = mg.GetHdc
    ' added code to compute and capture the form 
    ' title bar and borders 
    Dim widthDiff As Integer = _
       (myForm.Width - myForm.ClientRectangle.Width)
    Dim heightDiff As Integer = _
       (myForm.Height - myForm.ClientRectangle.Height)
    Dim borderSize As Integer = widthDiff \ 2
    Dim heightTitleBar As Integer = heightDiff - borderSize
    BitBlt(dc2, 0, 0, _
       myForm.ClientRectangle.Width + widthDiff, _
       myForm.ClientRectangle.Height + heightDiff, dc1, _
       0 - borderSize, 0 - heightTitleBar, 13369376)

    g.ReleaseHdc(dc1)
    mg.ReleaseHdc(dc2)
  End Function

  Private Declare Function BitBlt Lib "gdi32.dll" Alias _
   "BitBlt" (ByVal hdcDest As IntPtr, _
   ByVal nXDest As Integer, ByVal nYDest As _
   Integer, ByVal nWidth As Integer, _
   ByVal nHeight As Integer, ByVal _
   hdcSrc As IntPtr, ByVal nXSrc As Integer, _
   ByVal nYSrc As Integer, _
   ByVal dwRop As System.Int32) As Long


End Class
