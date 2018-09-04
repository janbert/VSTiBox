Imports System.Text.RegularExpressions
Public Class ExportOptions

  Dim mpdfDoc As PDFLibNet.PDFWrapper
  Dim mPdfFileName As String
  Dim mFilter As String = ""
  Dim mPassword As String = ""

  Const BW_TIFF_G4 As String = "tiffg4"
  Const BW_TIFF_LZW As String = "tifflzw"
  Const GRAY_TIFF_NC As String = "tiffgray"
  Const GRAY_PNG = "pnggray"
  Const GRAY_JPG = "jpeggray"
  Const COLOR_TIFF_RGB As String = "tiff24nc"
  Const COLOR_TIFF_CMYK As String = "tiff32nc"
  Const COLOR_TIFF_CMYK_SEP As String = "tiffsep"
  Const COLOR_PNG_RGB As String = "png16m"
  Const COLOR_JPEG = "jpeg"

  Public Sub New(ByVal pdfFileName As String, ByVal pdfDoc As PDFLibNet.PDFWrapper, Optional ByVal password As String = "")

    ' This call is required by the Windows Form Designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    mPdfFileName = pdfFileName
    mpdfDoc = pdfDoc
    mPassword = password
    nuStart.Maximum = mpdfDoc.PageCount
    nuStart.Value = 1
    nuDown.Maximum = mpdfDoc.PageCount
    nuDown.Value = mpdfDoc.PageCount
    nuDPI.Value = 150
    SaveFileDialog1.Filter = rbPostscript.Tag
  End Sub

  Private Sub btOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btOK.Click
    If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
      Windows.Forms.Cursor.Current = Windows.Forms.Cursors.WaitCursor
      Dim filename As String = SaveFileDialog1.FileName
      If filename.EndsWith(".ps") Then
        mpdfDoc.PrintToFile(filename, nuStart.Value, nuDown.Value)
      ElseIf filename.EndsWith(".jpg") Then
        ConvertPDF.PDFConvert.ConvertPdfToGraphic(mPdfFileName, SaveFileDialog1.FileName, COLOR_JPEG, nuDPI.Value, nuStart.Value, nuDown.Value, False, mPassword)
      ElseIf filename.EndsWith(".tif") Then
        Dim returnFileName As String
        returnFileName = ConvertPDF.PDFConvert.ConvertPdfToGraphic(mPdfFileName, SaveFileDialog1.FileName, COLOR_TIFF_RGB, nuDPI.Value, nuStart.Value, nuDown.Value, False, mPassword)
        Try
          ImageUtil.CompressTiff(returnFileName)
        Catch
          MsgBox("An error occurred while applying LZW compression to the TIFF file. The TIFF file has been saved in an uncompressed format instead.", MsgBoxStyle.OkOnly, "TIFF Compression Error")
        End Try
      ElseIf filename.EndsWith(".png") Then
        ConvertPDF.PDFConvert.ConvertPdfToGraphic(mPdfFileName, SaveFileDialog1.FileName, COLOR_PNG_RGB, nuDPI.Value, nuStart.Value, nuDown.Value, False, mPassword)
      ElseIf filename.EndsWith(".txt") Then
        mpdfDoc.ExportText(filename, nuStart.Value, nuDown.Value, True, True)
      ElseIf filename.EndsWith(".html") And rbHtml.Checked Then
        mpdfDoc.ExportHtml(filename, nuStart.Value, nuDown.Value, True, True, False)
      ElseIf filename.EndsWith(".html") And rbHtmlImage.Checked Then
        ExportHTMLImages(filename)
      End If
      Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
    End If
    Me.Hide()
  End Sub

  Private Sub nuStart_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nuStart.ValueChanged
    If nuStart.Value > nuDown.Value Then
      nuStart.Value = nuDown.Value
    End If
  End Sub

  Private Sub nuDown_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nuDown.ValueChanged
    If nuDown.Value < nuStart.Value Then
      nuDown.Value = nuStart.Value
    End If
  End Sub

  Private Sub CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbHtml.CheckedChanged, rbJpeg.CheckedChanged, rbPostscript.CheckedChanged, rbText.CheckedChanged, rbPNG.CheckedChanged, rbTIFF.CheckedChanged, rbHtmlImage.CheckedChanged
    If rbHtml.Checked Then
      SaveFileDialog1.Filter = rbHtml.Tag
      GroupBox3.Enabled = False
    ElseIf rbHtmlImage.Checked Then
      SaveFileDialog1.Filter = rbHtmlImage.Tag
      GroupBox3.Enabled = True
    ElseIf rbJpeg.Checked Then
      SaveFileDialog1.Filter = rbJpeg.Tag
      GroupBox3.Enabled = True
    ElseIf rbPostscript.Checked Then
      SaveFileDialog1.Filter = rbPostscript.Tag
      GroupBox3.Enabled = False
    ElseIf rbText.Checked Then
      SaveFileDialog1.Filter = rbText.Tag
      GroupBox3.Enabled = False
    ElseIf rbPNG.Checked Then
      SaveFileDialog1.Filter = rbPNG.Tag
      GroupBox3.Enabled = True
    ElseIf rbTIFF.Checked Then
      SaveFileDialog1.Filter = rbTIFF.Tag
      GroupBox3.Enabled = True
    End If
  End Sub

  Private Sub btCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btCancel.Click
    Me.Hide()
  End Sub

  Private Sub ExportHTMLImages(ByVal fileName As String)
    Dim folderPath As String = System.Text.RegularExpressions.Regex.Replace(fileName, "(^.+\\).+$", "$1")
    Dim contentFolder As String = folderPath & "content"
    Dim imagesFolder As String = contentFolder & "\images"

    Dim topFrame As String = My.Resources.TopHtml
    topFrame = Regex.Replace(topFrame, "\{DocumentName\}", "<center><h2>" & Regex.Replace(mPdfFileName, "^.+\\", "") & "</h2></center>")

    Dim sideFrame As String = My.Resources.BookmarkHtml
    'Possible to allow some export from GhostScript renderer
    sideFrame = Regex.Replace(sideFrame, "\{Body\}", AFPDFLibUtil.BuildHTMLBookmarks(mpdfDoc))

    Dim pageFrame As String = My.Resources.PageHtml
    Dim mainPage As String = My.Resources.FrameHtml
    Dim pageSize As String = My.Resources.PagesizeHtml

    Dim di As System.IO.DirectoryInfo
    di = New System.IO.DirectoryInfo(contentFolder)
    If (Not di.Exists) Then
      di.Create()
    End If

    di = New System.IO.DirectoryInfo(imagesFolder)
    If (Not di.Exists) Then
      di.Create()
    End If

    Dim sw As New IO.StreamWriter(fileName, False)
    sw.Write(mainPage)
    sw.Close()

    Dim sw1 As New IO.StreamWriter(contentFolder & "\top.html", False)
    sw1.Write(topFrame)
    sw1.Close()

    Dim sw2 As New IO.StreamWriter(contentFolder & "\bookmark.html", False)
    sw2.Write(sideFrame)
    sw2.Close()

    Dim sw3 As New IO.StreamWriter(contentFolder & "\page.html", False)
    sw3.Write(pageFrame)
    sw3.Close()

    Dim sw4 As New IO.StreamWriter(contentFolder & "\pagesize.html", False)
    sw4.Write(pageSize)
    sw4.Close()

    ConvertPDF.PDFConvert.ConvertPdfToGraphic(mPdfFileName, imagesFolder & "\page.png", COLOR_PNG_RGB, nuDPI.Value, nuStart.Value, nuDown.Value, False, mPassword)
  End Sub

End Class