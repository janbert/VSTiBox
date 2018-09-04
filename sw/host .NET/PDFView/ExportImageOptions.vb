Imports System.Windows.Forms

Public Class ExportImageOptions

  Dim mImgFileNames As String()
  Dim mFilter As String = ""
  Dim IsMultiple As Boolean = False
  Public SavedFileName As String = ""

  Public Sub New(ByVal imgFileNames As String())

    ' This call is required by the Windows Form Designer.
    InitializeComponent()

    If imgFileNames.Length = 1 Then
      Dim pageCount As Integer = ImageUtil.GetImageFrameCount(imgFileNames(0))
      If pageCount > 1 Then
        nuStart.Maximum = pageCount
        nuStart.Value = 1
        nuDown.Maximum = pageCount
        nuDown.Value = pageCount
      Else
        DisablePageRange()
      End If
    Else
      DisablePageRange()
    End If
    mImgFileNames = imgFileNames
    SaveFileDialog1.Filter = rbPDF.Tag
  End Sub

  Private Sub DisablePageRange()
    nuStart.Maximum = 0
    nuStart.Value = 0
    nuDown.Maximum = 0
    nuDown.Value = 0
    GroupBox2.Visible = False
  End Sub

  Private Sub ExportImageOptions_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    LoadPageSizes()
    LoadLanguages()
  End Sub

  Private Sub LoadPageSizes()
    Dim hash As New List(Of DictionaryEntry)
    hash.Add(New DictionaryEntry("Letter", iTextSharp.text.PageSize.LETTER))
    hash.Add(New DictionaryEntry("Legal", iTextSharp.text.PageSize.LEGAL))
    hash.Add(New DictionaryEntry("Ledger", iTextSharp.text.PageSize._11X17))
    hash.Add(New DictionaryEntry("Executive", iTextSharp.text.PageSize.EXECUTIVE))
    hash.Add(New DictionaryEntry("A4", iTextSharp.text.PageSize.A4))
    hash.Add(New DictionaryEntry("B4", iTextSharp.text.PageSize.B4))
    hash.Add(New DictionaryEntry("A3", iTextSharp.text.PageSize.A3))
    cbPageSize.DataSource = hash
    cbPageSize.DisplayMember = "Key"
  End Sub

  Private Sub LoadLanguages()
    'Make sure all of the language files are in the \tessdata directory
    Dim hash As New List(Of DictionaryEntry)
    hash.Add(New DictionaryEntry("English", TesseractOCR.Language.English))
    'hash.Add(New DictionaryEntry("Basque", TesseractOCR.Language.Basque))
    'hash.Add(New DictionaryEntry("Dutch", TesseractOCR.Language.Dutch))
    'hash.Add(New DictionaryEntry("Fraktur", TesseractOCR.Language.Fraktur))
    'hash.Add(New DictionaryEntry("French", TesseractOCR.Language.French))
    'hash.Add(New DictionaryEntry("German", TesseractOCR.Language.German))
    'hash.Add(New DictionaryEntry("Italian", TesseractOCR.Language.Italian))
    'hash.Add(New DictionaryEntry("Portuguese", TesseractOCR.Language.Portuguese))
    'hash.Add(New DictionaryEntry("Spanish", TesseractOCR.Language.Spanish))
    'hash.Add(New DictionaryEntry("Vietnamese", TesseractOCR.Language.Vietnamese))
    cbLanguage.DataSource = hash
    cbLanguage.DisplayMember = "Key"
  End Sub

  Private Sub btCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btCancel.Click
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

  Private Sub btOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btOK.Click
    If SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
      Windows.Forms.Cursor.Current = Windows.Forms.Cursors.WaitCursor
      Dim filename As String = SaveFileDialog1.FileName
      If filename.EndsWith(".pdf") Then 
        iTextSharpUtil.GraphicListToPDF(mImgFileNames, SaveFileDialog1.FileName, cbPageSize.SelectedValue.Value, IIf(cbOCR.Checked, cbLanguage.SelectedValue.Value, ""), nuStart.Value, nuDown.Value, tbUserPass.Text, tbOwnerPass.Text)
        SavedFileName = SaveFileDialog1.FileName
      End If
      Windows.Forms.Cursor.Current = Windows.Forms.Cursors.Default
    End If
    Me.Hide()
  End Sub

End Class